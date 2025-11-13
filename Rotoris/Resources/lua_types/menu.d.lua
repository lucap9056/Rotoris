--- A Lua module for managing menus within the Rotoris application.
--- @class Rotoris.LuaMenu
--- @field open_menu fun(self: Rotoris.LuaMenu, name: string) Opens a menu by name.
--- @field set_options fun(self: Rotoris.LuaMenu, optionsJson: string, idleTimeout?: integer) Sets the options for the current menu.
--- @field close_menu fun(self: Rotoris.LuaMenu) Closes the currently active menu.
--- @field set_size fun(self: Rotoris.LuaMenu, size: integer) Sets the size of the menu.
--- @field set_temporary_scroll fun(self: Rotoris.LuaMenu, clockwiseModuleName: string, counterclockwiseModuleName: string): boolean Sets temporary scroll options for the menu.
--- @field set_temporary_scroll fun(self: Rotoris.LuaMenu, temporary_scroll: Rotoris.LuaMenu.TemporaryScroll): boolean Sets temporary scroll options for the menu.
--- @field print_message fun(self: Rotoris.LuaMenu, message: string, durationSeconds?: integer) Displays a message.
--- @field create_menu fun(self: Rotoris.LuaMenu): Rotoris.LuaMenu.MenuEditor Creates a new MenuEditor instance for building menu options fluently.
--- @see Rotoris.TemporaryScroll
--- @see menu.MenuEditor

--- @class Rotoris.LuaMenu.TemporaryScroll
--- @field IdleTimeout? integer Optional: The number of seconds before the scroll action times out (must be > 0 if ClickModuleName is set).
--- @field ClockwiseModuleName? string Optional: The name of the module to execute when clockwise scroll is detected.
--- @field CounterclockwiseModuleName? string Optional: The name of the module to execute when counterclockwise scroll is detected.
--- @field ClickModuleName? string Optional: The name of the module to execute on a click event.
--- @field TimeoutModuleName? string Optional: The name of the module to execute upon a timeout.

--- A fluent interface for building menu options in a structured way.
--- @class Rotoris.LuaMenu.MenuEditor
--- @field add_option fun(self: Rotoris.LuaMenu.MenuEditor, id: string, action_id?: string, icon_path?: string): Rotoris.LuaMenu.MenuEditor Adds a menu option.
--- @field to_json fun(self: Rotoris.LuaMenu.MenuEditor): string Serializes the menu options to a JSON string.

--- @class Rotoris.LuaMenu
local menu = {}

menu:open_menu("main_menu")

menu:close_menu()

menu:set_size(500)

menu:set_temporary_scroll({
    IdleTimeout = 10,
    ClockwiseModuleName = "clockwise",
    CounterclockwiseModuleName = "counterclock",
    ClickModuleName = "click",
    TimeoutModuleName = "timeout"
})

menu:print_message("Welcome to the Rotoris Menu!", 5)

local menuEditor = menu:create_menu()
menuEditor:add_option("option1", "action1", "icon1.png")
          :add_option("option2", "action2", "icon2.png")

local menuJson = menuEditor:to_json()

menu:set_options(menuJson, 15)