using NLua.Exceptions;
using Rotoris.LuaModules;
using Rotoris.LuaModules.LuaCanvas;
using Rotoris.LuaModules.LuaMedia;
using Rotoris.Logger;
using RotorisLib;

namespace Rotoris
{
    public class LuaModuleSearcher(Dictionary<string, ActionModule> cachedModules)
    {
        public string? search(string name)
        {
            if (cachedModules.TryGetValue(name, out ActionModule module))
            {
                return module.Script;
            }
            return null;
        }
    }
    public class LuaEnvironment(Configuration configuration)
    {
        public double UiSize { get; set; } = configuration.UiSize;
        public string ModuleName { get; set; } = "";
    }
    internal class LuaRunner(int min = 1, int max = 10)
    {
        public static readonly Dictionary<string, string> BuiltInModules = new()
        {
            {"SkiaSharp", LuaCanvas.Module },
        };
        private readonly MediaSessionsManager mediaSessionsManager = new();
        private readonly LuaLog log = new();
        private readonly LuaSystem sys = new();
        private readonly LuaHID hid = new();
        private readonly LuaWindows windows = new();
        private readonly LuaMenu menu = new();
        private readonly LuaFileSystem fs = new();
        private readonly LuaAudio audio = new();
        private readonly LuaCache cache = new();
        private readonly LuaVmPool pool = new(min, max);
        private LuaModuleSearcher luaModuleSearcher = new([]);
        private bool isInitialized = false;
        public async void Initialize(Dictionary<string, ActionModule> cachedModules, Configuration configuration)
        {
            if (isInitialized)
            {
                return;
            }
            await mediaSessionsManager.Initialize();

            luaModuleSearcher = new LuaModuleSearcher(cachedModules);

            pool.LuaVmCreated += (sender, e) =>
            {
                e.Vm["module_searcher"] = luaModuleSearcher;
                e.Vm["log"] = log;
                e.Vm["sys"] = sys;
                e.Vm["hid"] = hid;
                e.Vm["windows"] = windows;
                e.Vm["menu"] = menu;
                e.Vm["audio"] = audio;
                e.Vm["cache"] = cache;
                e.Vm["fs"] = fs;
                e.Vm["media"] = new LuaMedia(mediaSessionsManager);
                e.Vm["canvas"] = new LuaCanvas(400);
                e.Vm["env"] = new LuaEnvironment(configuration);

                e.Vm.LoadCLRPackage();
                e.Vm.DoString(@"
                local function rotoris_module_loader(modname)
                    local module_string = module_searcher:search(modname)
                    if module_string == nil then
                        return nil
                    end
                    return load(module_string)
                end

                package['searchers'] = { rotoris_module_loader }

                function run(script_name)
                    local script_string = module_searcher:search(script_name)
                    local script_function, error_msg = load(script_string, ""@"" .. script_name)
                    if not script_function then
                        error(""Syntax error in script '"" .. script_name .. ""': "" .. error_msg)
                    end

                    local success, result = pcall(script_function)

                    if not success then
                        log:println(""Execution error in script '"" .. script_name .. ""': "" .. result)
                        error(""Execution error in script '"" .. script_name .. ""': "" .. result)
                    end

                end
                ");
            };

            isInitialized = true;
        }
        public void Clear()
        {
            if (!isInitialized)
            {
                return;
            }
            pool.ForEach((vm, isDynamic) =>
            {
                LuaCanvas? canvas = vm["canvas"] as LuaCanvas;
                canvas?.Pause();
            });
            cache.clear();
        }
        public bool Run(string moduleName)
        {
            if (!isInitialized)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(moduleName))
            {
                Log.Error("The provided Lua script is empty or null. Execution aborted.");
                return true;
            }

            Task.Run(() =>
            {
                pool.UseLua((vm) =>
                {

                    if (vm["env"] is LuaEnvironment env)
                    {
                        env.ModuleName = moduleName;
                    }

                    try
                    {
                        vm.DoString($"run('{moduleName}')");
                    }
                    catch (LuaException ex)
                    {
                        Log.Error($"[LUA] An error occurred during script execution: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[FATAL] An unexpected error occurred: {ex.Message}");
                    }
                });
            });

            return true;
        }
        public void Dispose()
        {
            cache.clear();
            pool.Dispose();
            audio.Dispose();
        }
    }
}