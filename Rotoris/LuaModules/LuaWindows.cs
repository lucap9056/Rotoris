using System.Text.RegularExpressions;
using Rotoris.LuaModules.LuaUtils;
using Rotoris.Logger;
using System.Text;
using NLua;

namespace Rotoris.LuaModules
{
    /*
--- Provides functionalities to interact with Windows OS windows, such as focusing, maximizing, minimizing, and retrieving window information.
--- @class Rotoris.LuaWindows
--- @field focus_window fun(self:Rotoris.LuaWindows, windowTitle: string): Rotoris.Result<boolean> Focuses a window with the specified title.
--- @field get_active_window_title fun(self:Rotoris.LuaWindows): Rotoris.Result<string?> Retrieves the title of the currently active window.
--- @field maximize_window fun(self:Rotoris.LuaWindows, windowTitle: string): Rotoris.Result<boolean> Maximizes a window with the specified title.
--- @field minimize_window fun(self:Rotoris.LuaWindows, windowTitle: string): Rotoris.Result<boolean> Minimizes a window with the specified title.
--- @field get_window_size fun(self:Rotoris.LuaWindows, windowTitle: string, emptyTable: table): Rotoris.Result<Rotoris.LuaWindows.WindowSize> Retrieves the size and position of a window with the specified title and populates the provided Lua table.
--- @field get_windows fun(self:Rotoris.LuaWindows, emptyTable: table): Rotoris.Result<table> Populates the provided Lua table with the titles of all open windows.
--- @field find_window_regex fun(self:Rotoris.LuaWindows, pattern: string): Rotoris.Result<string?> Finds a window title matching the specified regular expression pattern.

--- @class Rotoris.LuaWindows.WindowSize
--- @field Top number The top position of the window.
--- @field Left number The left position of the window.
--- @field Right number The right position of the window.
--- @field Bottom number The bottom position of the window.
--- @field Width number The width of the window.
--- @field Height number The height of the window.
     */
    public class LuaWindows
    {
        public Result<bool> focus_window(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle))
            {
                return Result<bool>.Err("Window title cannot be empty.");
            }

            try
            {
                IntPtr windowHandle = User32.FindWindow(null, windowTitle);
                if (windowHandle != IntPtr.Zero)
                {
                    if (User32.SetForegroundWindow(windowHandle))
                    {
                        return Result<bool>.Ok(true);
                    }
                    else
                    {
                        string error = $"Failed to set foreground window for '{windowTitle}'.";
                        return Result<bool>.Err(error);
                    }
                }
                else
                {
                    string error = $"Window with title '{windowTitle}' was not found.";
                    return Result<bool>.Err(error);
                }
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while trying to focus window '{windowTitle}'. Reason: {ex.Message}";
                return Result<bool>.Err(error);
            }
        }
        public Result<string?> get_active_window_title()
        {
            try
            {
                nint hWnd = User32.GetForegroundWindow();
                if (hWnd != nint.Zero)
                {
                    const int nChars = 256;
                    StringBuilder buff = new(nChars);

                    if (User32.GetWindowText(hWnd, buff, nChars) > 0)
                    {
                        string title = buff.ToString().Trim();
                        if (!string.IsNullOrEmpty(title))
                        {
                            return Result<string?>.Ok(title);
                        }
                    }
                }

                return Result<string?>.Ok(null);
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while getting the active window title. Reason: {ex.Message}";
                return Result<string?>.Err(error);
            }
        }
        public Result<bool> maximize_window(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle))
            {
                return Result<bool>.Err("Window title cannot be empty.");
            }

            try
            {
                nint windowHandle = User32.FindWindow(null, windowTitle);
                if (windowHandle != nint.Zero)
                {
                    if (User32.ShowWindow(windowHandle, User32.SW_MAXIMIZE))
                    {
                        return Result<bool>.Ok(true);
                    }
                    else
                    {
                        string error = $"Failed to maximize window '{windowTitle}'.";
                        return Result<bool>.Err(error);
                    }
                }
                else
                {
                    string error = $"Window with title '{windowTitle}' was not found.";
                    return Result<bool>.Err(error);
                }
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while maximizing window '{windowTitle}'. Reason: {ex.Message}";
                return Result<bool>.Err(error);
            }
        }
        public Result<bool> minimize_window(string windowTitle)
        {
            if (string.IsNullOrWhiteSpace(windowTitle))
            {
                return Result<bool>.Err("Window title cannot be empty.");
            }

            try
            {
                nint windowHandle = User32.FindWindow(null, windowTitle);
                if (windowHandle != nint.Zero)
                {
                    if (User32.ShowWindow(windowHandle, User32.SW_MINIMIZE))
                    {
                        return Result<bool>.Ok(true);
                    }
                    else
                    {
                        string error = $"Failed to minimize window '{windowTitle}'.";
                        return Result<bool>.Err(error);
                    }
                }
                else
                {
                    string error = $"Window with title '{windowTitle}' was not found.";
                    return Result<bool>.Err(error);
                }
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while minimizing window '{windowTitle}'. Reason: {ex.Message}";
                return Result<bool>.Err(error);
            }
        }
        public Result<LuaTable> get_window_size(string windowTitle, LuaTable luaTable)
        {
            if (string.IsNullOrWhiteSpace(windowTitle))
            {
                return Result<LuaTable>.Err("Window title cannot be empty.");
            }

            try
            {
                IntPtr windowHandle = User32.FindWindow(null, windowTitle);
                if (windowHandle == IntPtr.Zero)
                {
                    return Result<LuaTable>.Err($"Window with title '{windowTitle}' was not found.");
                }

                if (User32.GetWindowRect(windowHandle, out User32.RECT rect))
                {
                    luaTable["Top"] = rect.Top;
                    luaTable["Left"] = rect.Left;
                    luaTable["Right"] = rect.Right;
                    luaTable["Bottom"] = rect.Bottom;
                    luaTable["Width"] = rect.Right - rect.Left;
                    luaTable["Height"] = rect.Bottom - rect.Top;
                    return Result<LuaTable>.Ok(luaTable);
                }
                else
                {
                    return Result<LuaTable>.Err($"Failed to get rectangle for window '{windowTitle}'.");
                }
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while getting window size and position for '{windowTitle}'. Reason: {ex.Message}";
                return Result<LuaTable>.Err(error);
            }
        }
        public Result<LuaTable> get_windows(LuaTable luaTable)
        {
            int index = 1;

            bool callback(nint hWnd, nint lParam)
            {
                try
                {
                    if (User32.IsWindowVisible(hWnd))
                    {
                        const int nChars = 256;
                        StringBuilder Buff = new(nChars);

                        if (User32.GetWindowText(hWnd, Buff, nChars) > 0)
                        {
                            string title = Buff.ToString().Trim();

                            if (!string.IsNullOrEmpty(title))
                            {
                                luaTable[index] = title;
                                index++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning($"An error occurred in EnumWindows callback. Reason: {ex.Message}");
                }

                return true;
            }

            try
            {
                User32.EnumWindows(callback, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while enumerating windows. Reason: {ex.Message}";
                return Result<LuaTable>.Err(error);
            }

            return Result<LuaTable>.Ok(luaTable);
        }

        public Result<string?> find_window_regex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return Result<string?>.Err("Regular expression pattern cannot be empty.");
            }

            try
            {
                Regex regex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Result<string?> result = Result<string?>.Ok(null);

                bool stopEnumerating = false;

                bool callback(nint hWnd, nint lParam)
                {
                    if (stopEnumerating)
                    {
                        return false;
                    }

                    if (User32.IsWindowVisible(hWnd))
                    {
                        const int nChars = 256;
                        StringBuilder Buff = new(nChars);

                        if (User32.GetWindowText(hWnd, Buff, nChars) > 0)
                        {
                            string title = Buff.ToString().Trim();

                            if (!string.IsNullOrEmpty(title))
                            {
                                if (regex.IsMatch(title))
                                {
                                    stopEnumerating = true;
                                    result = Result<string?>.Ok(title);
                                    return false;
                                }
                            }
                        }
                    }

                    return true;
                }

                User32.EnumWindows(callback, IntPtr.Zero);

                return result;
            }
            catch (ArgumentException ex)
            {
                string error = $"Invalid regex pattern '{pattern}'. Reason: {ex.Message}";
                return Result<string?>.Err(error);
            }
            catch (Exception ex)
            {
                string error = $"An error occurred while trying to find window by regex. Reason: {ex.Message}";
                return Result<string?>.Err(error);
            }
        }
    }
}
