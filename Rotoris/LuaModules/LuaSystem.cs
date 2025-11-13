using System.Diagnostics;
using Rotoris.Logger;

namespace Rotoris.LuaModules
{
    /*
--- Provides system-level functionalities such as executing commands and opening files or URLs.
--- @class Rotoris.LuaSystem
--- @field exec fun(self:Rotoris.LuaSystem, command: string, ...: string): Process? Executes a system command or opens a file with optional arguments.
--- @field open fun(self:Rotoris.LuaSystem, targetPath: string) Opens a file or URL with the default associated application.

---@class Process
     */
    public class LuaSystem
    {
        public Process? exec(string command, params string[] arguments)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                Log.Error("Command or file path cannot be empty.");
                return null;
            }

            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = command,
                    UseShellExecute = true
                };

                if (arguments != null && arguments.Length > 0)
                {
                    startInfo.Arguments = string.Join(" ", arguments);
                }

                Log.Info($"Executing: '{startInfo.FileName}' with arguments: '{startInfo.Arguments}'");
                Process? process = Process.Start(startInfo);

                if (process != null)
                {
                    Log.Info($"Successfully started process for '{command}'.");
                }
                else
                {
                    Log.Warning($"The process for '{command}' failed to start, returned null.");
                }

                return process;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Log.Error($"Failed to execute '{command}'. Reason: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected error occurred while executing '{command}'. Reason: {ex.Message}");
            }

            return null;
        }

        public void open(string targetPath)
        {

            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Log.Error("The path or URL to open cannot be empty.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(targetPath)
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                Log.Info($"Successfully requested to open '{targetPath}'.");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Log.Error($"Failed to open '{targetPath}'. Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected error occurred while trying to open '{targetPath}'. Details: {ex.Message}");
            }
        }
    }
}
