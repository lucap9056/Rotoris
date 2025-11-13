namespace RotorisLib
{
    public struct Configuration
    {
        /// <summary>Default key codes.</summary>
        public static class Default
        {
            public static readonly double UiSize = 400;
            public static readonly System.Windows.Media.Color? UiBackground = null;
            public static readonly System.Windows.Media.Color? UiForeground = null;
            public static readonly System.Windows.Media.Color? UiAccent = null;
            public static readonly Hotkey PrimaryKey = new(0xAD);             // VK_VOLUME_MUTE (Default hotkey to open menu)
            public static readonly Hotkey ClockwiseKey = new(0xAF);           // VK_VOLUME_UP (Default hotkey for clockwise movement)
            public static readonly Hotkey CounterclockwiseKey = new(0xAE);    // VK_VOLUME_DOWN (Default hotkey for counter-clockwise movement)
        }
        /// <summary>INI file section names.</summary>
        public static class Sections
        {
            public static readonly string Appearance = "Appearance";
            public static readonly string KeyBindings = "Key_Bindings";
        }
        /// <summary>INI file column names within sections.</summary>
        public static class SectionColumns
        {
            /// <summary>Appearance section column names.</summary>
            public static class Appearance
            {
                public static readonly string UiSize = "Size";
                public static readonly string BackgroundColor = "Background_Color";
                public static readonly string ForegroundColor = "Foreground_Color";
                public static readonly string AccentColor = "Accent_Color";
            }
            /// <summary>KeyBindings section column names.</summary>
            public static class KeyBindings
            {
                public static readonly string PrimaryKey = "Primary_Key";
                public static readonly string ClockwiseKey = "Clockwise_Key";
                public static readonly string CounterclockwiseKey = "Counterclockwise_Key";
            }
        }

        /// <summary>The size of the UI in pixels (clamped between 200 and 1200).</summary>
        public double UiSize { set; get; } = Default.UiSize;
        /// <summary>The background color of the UI.</summary>
        public System.Windows.Media.Color? UiBackground { set; get; } = Default.UiBackground;
        /// <summary>The foreground color of the UI.</summary>
        public System.Windows.Media.Color? UiForeground { set; get; } = Default.UiForeground;
        /// <summary>The accent color of the UI.</summary>
        public System.Windows.Media.Color? UiAccent { set; get; } = Default.UiAccent;
        /// <summary>The virtual key code for the primary menu action.</summary>
        public Hotkey PrimaryKey { set; get; } = Default.PrimaryKey;
        /// <summary>The virtual key code for the clockwise menu action.</summary>
        public Hotkey ClockwiseKey { set; get; } = Default.ClockwiseKey;
        /// <summary>The virtual key code for the counter-clockwise menu action.</summary>
        public Hotkey CounterclockwiseKey { set; get; } = Default.CounterclockwiseKey;


        /// <summary>
        /// Reads application configuration settings from an INI file.
        /// </summary>
        /// <param name="ini">The <c>IniFile</c> instance to read from.</param>
        /// <returns>A <see cref="Configuration"/> struct populated with settings.</returns>
        public Configuration(IniFile ini)
        {
            var config = new Configuration
            {
                UiSize = 400,
                PrimaryKey = Default.PrimaryKey,
                ClockwiseKey = Default.ClockwiseKey,
                CounterclockwiseKey = Default.CounterclockwiseKey,
            };

            // Read UI Size
            if (ini.ReadValue(Sections.Appearance, SectionColumns.Appearance.UiSize, out string sizeString) && double.TryParse(sizeString, out var size))
            {
                config.UiSize = System.Math.Clamp(size, 200, 1200);
            }

            // Read UI Colors
            if (ini.ReadValue(Sections.Appearance, SectionColumns.Appearance.BackgroundColor, out string backgroundHexString))
            {
                TrySetMediaColor($"#{backgroundHexString}", (color) => config.UiBackground = color);
            }
            if (ini.ReadValue(Sections.Appearance, SectionColumns.Appearance.ForegroundColor, out string foregroundHexString))
            {
                TrySetMediaColor($"#{foregroundHexString}", (color) => config.UiForeground = color);
            }
            if (ini.ReadValue(Sections.Appearance, SectionColumns.Appearance.AccentColor, out string accentHexString))
            {
                TrySetMediaColor($"#{accentHexString}", (color) => config.UiAccent = color);
            }

            // Read Key Bindings
            if (ini.ReadValue(Sections.KeyBindings, SectionColumns.KeyBindings.PrimaryKey, out string primaryKeyString))
            {
                if (Hotkey.TryParse(primaryKeyString, out Hotkey primaryKey))
                {
                    config.PrimaryKey = primaryKey;
                }
            }

            if (ini.ReadValue(Sections.KeyBindings, SectionColumns.KeyBindings.ClockwiseKey, out string clockwiseKeyString))
            {
                if (Hotkey.TryParse(clockwiseKeyString, out Hotkey clockwiseKey))
                {
                    config.ClockwiseKey = clockwiseKey;
                }
            }

            if (ini.ReadValue(Sections.KeyBindings, SectionColumns.KeyBindings.CounterclockwiseKey, out string counterclockwiseKeyString))
            {
                if (Hotkey.TryParse(counterclockwiseKeyString, out Hotkey counterclockwiseKey))
                {
                    config.CounterclockwiseKey = counterclockwiseKey;
                }
            }

            UiSize = config.UiSize;
            UiAccent = config.UiAccent;
            UiBackground = config.UiBackground;
            UiForeground = config.UiForeground;
            PrimaryKey = config.PrimaryKey;
            ClockwiseKey = config.ClockwiseKey;
            CounterclockwiseKey = config.CounterclockwiseKey;
        }

        /// <summary>
        /// Attempts to parse a hexadecimal color string (with prepended #) into a <see cref="System.Windows.Media.Color"/> and set it via an action.
        /// </summary>
        /// <param name="hexString">The hexadecimal color string (e.g., "#FF000000").</param>
        /// <param name="setter">The action delegate to set the parsed color.</param>
        private static void TrySetMediaColor(string hexString, System.Action<System.Windows.Media.Color> setter)
        {
            try
            {
                object? colorObject = System.Windows.Media.ColorConverter.ConvertFromString(hexString);
                if (colorObject is System.Windows.Media.Color color)
                {
                    setter(color);
                }
            }
            catch (System.FormatException ex)
            {
                System.Diagnostics.Debug.WriteLine($"WARN: Failed to convert color string '{hexString}'. Format error: {ex.Message}");
            }
        }
    }
}

