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

--- @class Rotoris.LuaWindows
local windows = {}

windows:get_active_window_title():match({
    Ok = function(title)
        if title then
            print("Active window title: " .. title)
        else
            print("No active window.")
        end
    end,
    Err = function(errorMessage)
        print("Error retrieving active window title: " .. errorMessage)
    end
})

windows:focus_window("Untitled - Notepad"):match({
    Ok = function(success)
        if success then
            print("Window focused successfully.")
        else
            print("Failed to focus window.")
        end
    end,
    Err = function(errorMessage)
        print("Error focusing window: " .. errorMessage)
    end
})

windows:get_window_size("Untitled - Notepad", {}):match({
    Ok = function(size)
        print("Window Size - Top: " ..
        size.Top .. ", Left: " .. size.Left .. ", Width: " .. size.Width .. ", Height: " .. size.Height)
    end,
    Err = function(errorMessage)
        print("Error retrieving window size: " .. errorMessage)
    end
})

windows:find_window_regex(".*Notepad.*"):match({
    Ok = function(title)
        if title then
            print("Found window title matching regex: " .. title)
        else
            print("No window title matches the regex.")
        end
    end,
    Err = function(errorMessage)
        print("Error finding window with regex: " .. errorMessage)
    end
})

windows:get_windows({}):match({
    --- @param windowTitles string[]
    Ok = function(windowTitles)
        print("Open windows:")
        for _, title in ipairs(windowTitles) do
            print("- " .. title)
        end
    end,
    Err = function(errorMessage)
        print("Error retrieving open windows: " .. errorMessage)
    end
})

windows:maximize_window("Untitled - Notepad"):match({
    Ok = function(success)
        if success then
            print("Window maximized successfully.")
        else
            print("Failed to maximize window.")
        end
    end,
    Err = function(errorMessage)
        print("Error maximizing window: " .. errorMessage)
    end
})

windows:minimize_window("Untitled - Notepad"):match({
    Ok = function(success)
        if success then
            print("Window minimized successfully.")
        else
            print("Failed to minimize window.")
        end
    end,
    Err = function(errorMessage)
        print("Error minimizing window: " .. errorMessage)
    end
})
