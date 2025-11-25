using Newtonsoft.Json;
using Rotoris.Logger;
using NLua;

namespace Rotoris.LuaModules
{
    /*
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
     */
    public class LuaMenu
    {
        public static readonly string GlobalName = "menu";
        public void open_menu(string name)
        {
            EventAggregator.PublishShowMenu(name);
        }

        public void set_options(string optionsJson, int idleTimeoutInSeconds = 0)
        {
            try
            {
                var options = RotorisLib.MenuOptionData.ParseOptionsFromJson(optionsJson);
                EventAggregator.PublishSetMenu(options, idleTimeoutInSeconds);
            }
            catch (JsonSerializationException ex)
            {
                Log.Error($"Failed to deserialize menu options from JSON. Reason: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected error occurred while setting menu options. Reason: {ex.Message}");
            }
        }

        public void close_menu()
        {
            EventAggregator.PublishHideMenu();
        }

        public void set_size(int size)
        {
            EventAggregator.PublishUISetSize(size);
        }

        public bool set_temporary_scroll(string clockwiseModuleName, string counterclockwiseModuleName)
        {
            EventAggregator.PublishSetTemporaryScroll(0, clockwiseModuleName, counterclockwiseModuleName, "", "");
            return true;
        }

        /*
--- @class Rotoris.LuaMenu.TemporaryScroll
--- @field IdleTimeout? integer Optional: The number of seconds before the scroll action times out (must be > 0 if ClickModuleName is set).
--- @field ClockwiseModuleName? string Optional: The name of the module to execute when clockwise scroll is detected.
--- @field CounterclockwiseModuleName? string Optional: The name of the module to execute when counterclockwise scroll is detected.
--- @field ClickModuleName? string Optional: The name of the module to execute on a click event.
--- @field TimeoutModuleName? string Optional: The name of the module to execute upon a timeout.
         */
        public bool set_temporary_scroll(LuaTable table)
        {
            object timeoutValue = table["IdleTimeout"];
            int idleTimeoutInSeconds = 0;

            if (timeoutValue != null)
            {
                idleTimeoutInSeconds = Convert.ToInt32(timeoutValue);
            }
            string clockwiseModuleName = table["ClockwiseModuleName"] as string ?? "";
            string counterclockwiseModuleName = table["CounterclockwiseModuleName"] as string ?? "";
            string clickModuleName = table["ClickModuleName"] as string ?? "";
            string timeoutModuleName = table["TimeoutModuleName"] as string ?? "";

            if (idleTimeoutInSeconds <= 0 && !string.IsNullOrEmpty(clickModuleName))
            {
                Log.Error("When IdleTimeout is less than or equal to 0, ClickModuleName cannot be set.");
                return false;
            }

            EventAggregator.PublishSetTemporaryScroll(idleTimeoutInSeconds, clockwiseModuleName, counterclockwiseModuleName, clickModuleName, timeoutModuleName);
            return true;
        }
        public void print_message(string message, int duration = 0)
        {
            EventAggregator.PublishDisplayMessage(message, duration);
        }

        public MenuEditor create_menu()
        {
            return new MenuEditor();
        }

        /*
--- A fluent interface for building menu options in a structured way.
--- @class Rotoris.LuaMenu.MenuEditor
--- @field add_option fun(self: Rotoris.LuaMenu.MenuEditor, id: string, action_id?: string, icon_path?: string): Rotoris.LuaMenu.MenuEditor Adds a menu option.
--- @field to_json fun(self: Rotoris.LuaMenu.MenuEditor): string Serializes the menu options to a JSON string.
        */
        public class MenuEditor
        {
            private List<RotorisLib.MenuOptionData> options = [];

            public MenuEditor add_option(string id, string actionId = "", string iconPath = "")
            {
                options.Add(new RotorisLib.MenuOptionData
                {
                    Id = id,
                    IconPath = iconPath,
                    ActionId = actionId,
                });
                return this;
            }

            public string to_json()
            {
                return JsonConvert.SerializeObject(options, Formatting.Indented);
            }
        }
    }
}
