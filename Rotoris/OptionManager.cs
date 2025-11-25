using Newtonsoft.Json;
using RotorisLib;
using System.IO;
using Rotoris.MainViewer;
using Rotoris.Logger;

namespace Rotoris
{
    internal class OptionManager
    {
        private class TimeoutClock
        {
            private readonly int initialTimeoutInSeconds;
            private int remainingTimeoutInSeconds;
            private readonly Timer timer;
            private readonly Action timeoutCallback;
            public TimeoutClock(int timeoutInSeconds, Action callback)
            {
                if (timeoutInSeconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(timeoutInSeconds), "The timeout duration must be greater than 0 seconds.");
                }

                ArgumentNullException.ThrowIfNull(callback, nameof(callback));

                initialTimeoutInSeconds = timeoutInSeconds;
                remainingTimeoutInSeconds = timeoutInSeconds;
                timeoutCallback = callback;
                timer = new Timer(OnTimerTick, null, 0, 1000);
            }
            private void OnTimerTick(object? state)
            {
                remainingTimeoutInSeconds--;
                if (remainingTimeoutInSeconds <= 0)
                {
                    timer.Dispose();
                    timeoutCallback?.Invoke();
                }
            }
            public void Reset()
            {
                remainingTimeoutInSeconds = initialTimeoutInSeconds;
            }
            public void Cancel()
            {
                timer?.Dispose();
            }
        }

        private readonly MessageManager messageManager = new();
        private readonly Lock configLock = new();
        private LuaRunner runner = new();

        private Dictionary<string, MenuOptionData[]> cachedMenus = [];
        private Dictionary<string, ActionModule> cachedLuaModules = [];

        private MenuOptionData[] currentOptions = [];
        private TemporaryScrollConfig temporaryScroll = new() { Enabled = false };
        private TimeoutClock? idleTimeout;

        private bool hasVolumeUpModule = false;
        private bool hasVolumeDownModule = false;
        private int currentIndex = 0;
        private int totalCount = 0;
        private Configuration configuration;
        private EventHandler<GlobalInputHook.InputHookEventArgs>? inputDownHandler;

        public OptionManager(GlobalInputHook inputHook)
        {
            Directory.CreateDirectory(AppConstants.AppModuleDirectory);
            Directory.CreateDirectory(AppConstants.AppIconCacheDirectory);

            lock (configLock)
            {
                configuration = Load();
                inputDownHandler = OnInputDown(configuration);

                inputHook.KeyDown += inputDownHandler;
            }

            EventAggregator.ReloadReceived += (sender, e) =>
            {
                lock (configLock)
                {

                    inputHook.KeyDown -= inputDownHandler;

                    runner.Dispose();
                    runner = new LuaRunner();

                    configuration = Load();

                    inputDownHandler = OnInputDown(configuration);
                    inputHook.KeyDown += inputDownHandler;
                }
            };

            EventAggregator.ShowMenuReceived += OnShowMenu;
            EventAggregator.HideMenuReceived += OnHideMenu;
            EventAggregator.SetMenuReceived += OnSetMenu;
            EventAggregator.SetTemporaryScrollReceived += OnSetTemporaryScroll;
            EventAggregator.ClockwisePressReceived += OnClockwisePressed;
            EventAggregator.CounterclockwisePressReceived += OnCounterclockwisePressed;
            EventAggregator.DisplayMessageReceived += OnDisplayMessage;
            EventAggregator.NextOptionReceived += OnNextOption;
            EventAggregator.PreviousOptionReceived += OnPreviousOption;
            EventAggregator.ExecuteSelectedOptionReceived += OnExecuteSelectedOption;
            EventAggregator.ExecuteActionReceived += OnExecuteAction;
        }
        private Configuration Load()
        {
            string[] files = FileCollect.CollectRelativeFilePaths(AppConstants.AppModuleDirectory);
            var normalizeFiles = FileCollect.NormalizeFiles(AppConstants.AppModuleDirectory, files);
            cachedLuaModules = FileCollect.CreateFileCache(normalizeFiles, ".lua", (s) => new ActionModule(s));
            cachedMenus = FileCollect.CreateFileCache(normalizeFiles, ".json", MenuOptionData.ParseOptionsFromJson);

            foreach (string menu in FileCollect.CreateFileList(normalizeFiles, ".json"))
            {
                cachedLuaModules.Add($"OPEN_MENU-{menu}",
                    new ActionModule
                    {
                        CallNext = false,
                        Script = $"menu:open_menu('{menu}')",
                    });
                Log.Info($"OPEN_MENU-{menu}");
            }

            foreach (string action in AppConstants.ActionScriptsMap.Keys)
            {
                string script = AppConstants.ActionScriptsMap[action];
                cachedLuaModules[action] = new ActionModule(script);
            }

            if (ReadScript(AppConstants.BuiltInActionIds.Root, out string rootScript))
            {
                cachedLuaModules[AppConstants.BuiltInActionIds.Root] = new ActionModule(rootScript);
            }
            else
            {
                cachedLuaModules[AppConstants.BuiltInActionIds.Root] = new ActionModule(AppConstants.BuiltInActionScripts.Root);
            }

            if (ReadScript(AppConstants.BuiltInActionIds.Clockwise, out string clockwiseScript))
            {
                cachedLuaModules[AppConstants.BuiltInActionIds.Clockwise] = new ActionModule(clockwiseScript);
                hasVolumeUpModule = true;
            }
            else
            {
                hasVolumeUpModule = false;
            }

            if (ReadScript(AppConstants.BuiltInActionIds.Counterclockwise, out string counterclockwiseScript))
            {
                cachedLuaModules[AppConstants.BuiltInActionIds.Counterclockwise] = new ActionModule(counterclockwiseScript);
                hasVolumeDownModule = true;
            }
            else
            {
                hasVolumeDownModule = false;
            }

            if (ReadMenu(AppConstants.MenuNames.Root, out MenuOptionData[] rootOptions))
            {
                cachedMenus[AppConstants.MenuNames.Root] = rootOptions;
            }
            else
            {
                cachedMenus[AppConstants.MenuNames.Root] = AppConstants.BuiltInOptions.RootOptions;
            }

            foreach (string moduleName in LuaRunner.BuiltInModules.Keys)
            {
                string module = LuaRunner.BuiltInModules[moduleName];
                cachedLuaModules[moduleName] = new ActionModule(module);
            }

            var ini = new IniFile(AppConstants.AppConfigIniPath);
            var configuration = new Configuration(ini);

            EventAggregator.PublishUILoadConfiguration(configuration);

            runner.Initialize(cachedLuaModules, configuration);
            return configuration;
        }
        private static EventHandler<GlobalInputHook.InputHookEventArgs> OnInputDown(Configuration c)
        {
            return (sender, e) =>
            {
                if (e.Key.IsSupersetOf(c.ClockwiseKey))
                {
                    if (MainWindow.Status.IsMenuVisibled)
                    {
                        EventAggregator.PublishNextOption(e.ShouldBlock);
                    }
                    else
                    {
                        EventAggregator.PublishClockwisePress(e.ShouldBlock);
                    }
                    return;
                }

                if (e.Key.IsSupersetOf(c.CounterclockwiseKey))
                {
                    if (MainWindow.Status.IsMenuVisibled)
                    {
                        EventAggregator.PublishPreviousOption(e.ShouldBlock);
                    }
                    else
                    {
                        EventAggregator.PublishCounterclockwisePress(e.ShouldBlock);
                    }
                    return;
                }

                if (e.Key.IsSupersetOf(c.PrimaryKey))
                {
                    if (MainWindow.Status.IsMenuVisibled)
                    {
                        EventAggregator.PublishExecuteSelectedOption(e.ShouldBlock);
                    }
                    else
                    {
                        EventAggregator.PublishShowMenu();
                        e.ShouldBlock();
                    }
                    return;
                }
            };
        }
        private void OnHideMenu(object? sender, EventArgs e)
        {
            RemoveIdleTimeout();
            if (temporaryScroll.Enabled)
            {
                temporaryScroll.Enabled = false;
            }
            runner.Clear();
            messageManager.Clear();
            EventAggregator.PublishUISetSize(configuration.UiSize);
            EventAggregator.PublishUIClearMessageCanvas();
        }
        private void OnShowMenu(object? sender, EventAggregator.ShowMenuReceiveEventArgs e)
        {
            RemoveIdleTimeout();
            if (temporaryScroll.Enabled)
            {
                temporaryScroll.Enabled = false;
            }
            string menuName = e.Name;
            if (menuName == "")
            {
                runner.Run(AppConstants.BuiltInActionIds.Root);
                return;
            }

            Log.Info($"Displaying menu '{menuName}'");

            if (cachedMenus.TryGetValue(menuName, out var options))
            {
                EventAggregator.PublishSetMenu(options);
            }

        }
        private void OnTemporaryScrollTimeout()
        {
            RemoveIdleTimeout();
            if (temporaryScroll.Enabled)
            {
                temporaryScroll.Enabled = false;
                if (cachedLuaModules.ContainsKey(temporaryScroll.TimeoutModuleName))
                {
                    runner.Run(temporaryScroll.TimeoutModuleName);
                }
            }
        }
        private void OnSetMenu(object? sender, EventAggregator.SetMenuReceiveEventArgs e)
        {
            RemoveIdleTimeout();
            if (e.IdleTimeoutInSeconds == 0)
            {
                currentOptions = new MenuOptionData[e.Options.Length + 1];
                currentOptions[0] = AppConstants.BuiltInOptions.Close;
                e.Options.CopyTo(currentOptions, 1);
            }
            else
            {
                currentOptions = e.Options;
                if (temporaryScroll.Enabled && string.IsNullOrEmpty(temporaryScroll.ClickModuleName))
                {
                    idleTimeout = new TimeoutClock(e.IdleTimeoutInSeconds, OnTemporaryScrollTimeout);
                }
                else
                {
                    idleTimeout = new TimeoutClock(e.IdleTimeoutInSeconds, EventAggregator.PublishHideMenu);
                }
            }

            currentIndex = 0;
            totalCount = currentOptions.Length;

            EventAggregator.PublishUIDrawOptions(currentOptions);
        }
        private void OnSetTemporaryScroll(object? sender, EventAggregator.SetTemporaryScrollReceiveEventArgs e)
        {
            RemoveIdleTimeout();
            temporaryScroll = e.TemporaryScroll;

            if (e.TemporaryScroll.IdleTimeoutInSeconds > 0)
            {
                string clickModuleName = e.TemporaryScroll.ClickModuleName;

                MenuOptionData clickAction = string.IsNullOrEmpty(clickModuleName) ?
                    AppConstants.BuiltInOptions.Close :
                    new MenuOptionData
                    {
                        Id = clickModuleName,
                        IconPath = "",
                        ActionId = clickModuleName,
                    };

                MenuOptionData[] options = [clickAction];
                EventAggregator.PublishSetMenu(options, e.TemporaryScroll.IdleTimeoutInSeconds);
            }
            else
            {
                EventAggregator.PublishSetMenu([]);
            }
        }
        public void OnNextOption(object? sender, EventAggregator.KeyboardShouldBlockEventArgs e)
        {
            e.ShouldBlock();
            idleTimeout?.Reset();
            if (temporaryScroll.Enabled)
            {
                if (cachedLuaModules.ContainsKey(temporaryScroll.ClockwiseModuleName))
                {
                    runner.Run(temporaryScroll.ClockwiseModuleName);
                }
                return;
            }
            currentIndex = (currentIndex + 1) % totalCount;
            EventAggregator.PublishUIFocusOption(totalCount, currentIndex);
        }
        public void OnPreviousOption(object? sender, EventAggregator.KeyboardShouldBlockEventArgs e)
        {
            e.ShouldBlock();
            idleTimeout?.Reset();
            if (temporaryScroll.Enabled)
            {
                if (cachedLuaModules.ContainsKey(temporaryScroll.CounterclockwiseModuleName))
                {
                    runner.Run(temporaryScroll.CounterclockwiseModuleName);
                }
                return;
            }
            currentIndex = (totalCount + currentIndex - 1) % totalCount;
            EventAggregator.PublishUIFocusOption(totalCount, currentIndex);
        }
        public void OnExecuteSelectedOption(object? sender, EventAggregator.KeyboardShouldBlockEventArgs e)
        {
            idleTimeout?.Reset();
            if (temporaryScroll.Enabled)
            {
                string clickModuleName = temporaryScroll.ClickModuleName;
                if (!string.IsNullOrEmpty(clickModuleName) && cachedLuaModules.TryGetValue(clickModuleName, out var clickModule))
                {
                    runner.Run(clickModuleName);
                    if (!clickModule.CallNext)
                    {
                        e.ShouldBlock();
                    }
                }
                else
                {
                    EventAggregator.PublishHideMenu();
                }
                return;
            }

            MenuOptionData option = currentOptions[currentIndex];

            Log.Info($"Attempting to execute action for selected option ID: {option.ActionId}");

            if (!string.IsNullOrEmpty(option.ActionId) && cachedLuaModules.TryGetValue(option.ActionId, out var module))
            {
                runner.Run(option.ActionId);
                if (!module.CallNext)
                {
                    e.ShouldBlock();
                }
            }
        }
        public void OnClockwisePressed(object? sender, EventAggregator.KeyboardShouldBlockEventArgs e)
        {
            if (hasVolumeUpModule && cachedLuaModules.TryGetValue(AppConstants.BuiltInActionIds.Clockwise, out var module))
            {
                runner.Run(AppConstants.BuiltInActionIds.Clockwise);
                if (!module.CallNext)
                {
                    e.ShouldBlock();
                }
            }
        }

        public void OnCounterclockwisePressed(object? sender, EventAggregator.KeyboardShouldBlockEventArgs e)
        {
            if (hasVolumeDownModule && cachedLuaModules.TryGetValue(AppConstants.BuiltInActionIds.Counterclockwise, out var module))
            {
                runner.Run(AppConstants.BuiltInActionIds.Counterclockwise);
                if (!module.CallNext)
                {
                    e.ShouldBlock();
                }
            }
        }
        public void OnDisplayMessage(object? sender, EventAggregator.DisplayMessageReceiveEventArgs e)
        {
            messageManager.Add(e.Content, e.DurationSeconds);
        }

        private void OnExecuteAction(object? sender, EventAggregator.ExecuteActionEventArgs e)
        {
            string actionId = e.ActionId;
            if (!string.IsNullOrEmpty(actionId) && cachedLuaModules.ContainsKey(actionId))
            {
                runner.Run(actionId);
            }
        }

        private static bool ReadScript(string scriptName, out string content)
        {
            try
            {
                string scriptPath = Path.Combine(AppConstants.AppModuleDirectory, $"_{scriptName}.lua");

                if (File.Exists(scriptPath))
                {
                    Log.Info($"Script file found at '{scriptPath}'. Loading content...");
                    string scriptContent = File.ReadAllText(scriptPath);

                    Log.Info($"Successfully loaded script '{scriptName}'.");
                    content = scriptContent;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load script '{scriptName}'. Reason: {ex.Message}");
            }

            content = "";
            return false;
        }

        private static bool ReadMenu(string menuName, out MenuOptionData[] options)
        {
            string fileName = $"_{menuName}.json";
            try
            {
                string menuPath = Path.Combine(AppConstants.AppModuleDirectory, fileName);

                if (File.Exists(menuPath))
                {
                    Log.Info($"Menu file found at '{menuPath}'. Deserializing JSON...");
                    string menuJson = File.ReadAllText(menuPath);
                    MenuOptionData[] menuOptions = MenuOptionData.ParseOptionsFromJson(menuJson);

                    Log.Info($"Successfully loaded and deserialized menu '{menuName}'.");
                    options = menuOptions;
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                Log.Error($"Menu file '{fileName}' not found. Using default options.");
            }
            catch (JsonException ex)
            {
                Log.Error($"Failed to deserialize menu JSON for '{fileName}'. Reason: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected error occurred while loading menu '{fileName}'. Reason: {ex.Message}");
            }

            options = [];
            return false;
        }
        private void RemoveIdleTimeout()
        {
            if (idleTimeout != null)
            {
                idleTimeout.Cancel();
                idleTimeout = null;
            }
        }
        public void Dispose()
        {
            runner.Dispose();
        }
        private class MessageManager
        {
            private readonly Queue<Message> messages = new();
            private readonly Timer timer;
            private bool isDisplaying = false;
            public MessageManager()
            {
                timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            }

            public void Add(string content, int durationSeconds)
            {
                Message message = new Message
                {
                    Content = content,
                    DurationSeconds = durationSeconds
                };

                if (!isDisplaying)
                {
                    EventAggregator.PublishUIDisplayMessage(content);
                    if (message.DurationSeconds > 0)
                    {
                        isDisplaying = true;
                        timer.Change(message.DurationSeconds * 1000, Timeout.Infinite);
                    }
                }
                else
                {
                    if (message.DurationSeconds > 0)
                    {
                        messages.Enqueue(message);
                    }
                }
            }

            public void Clear()
            {
                messages.Clear();
                EventAggregator.PublishUIDisplayMessage();
                isDisplaying = false;
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            private void TimerCallback(object? state)
            {
                if (messages.Count > 0)
                {
                    Message nextMessage = messages.Dequeue();
                    isDisplaying = true;
                    EventAggregator.PublishUIDisplayMessage(nextMessage.Content);
                    if (nextMessage.DurationSeconds > 0)
                    {
                        timer.Change(nextMessage.DurationSeconds * 1000, Timeout.Infinite);
                    }
                }
                else
                {
                    EventAggregator.PublishUIDisplayMessage();
                    isDisplaying = false;
                }
            }

            private struct Message
            {
                public string Content { set; get; }
                public int DurationSeconds { set; get; }
            }
        }
    }
}
