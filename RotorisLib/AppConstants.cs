namespace RotorisLib
{
    /// <summary>
    /// Provides global application constants, configuration structures, utility methods, and built-in menu options.
    /// </summary>
    public static class AppConstants
    {
        // --- Application Directory and File Paths ---

        /// <summary>The name of the application.</summary>
        public static readonly string AppName = "Rotoris";
        /// <summary>The base directory for the application data, typically in MyDocuments.</summary>
        public static readonly string AppDataDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppName);
#if DEBUG
        /// <summary>The configuration directory (uses a 'DEBUG' subfolder in debug builds).</summary>
        public static readonly string AppConfigDirectory = System.IO.Path.Combine(AppDataDirectory, "DEBUG");
#else
        /// <summary>The configuration directory (same as AppDataDirectory in release builds).</summary>
        public static readonly string AppConfigDirectory = AppDataDirectory;
#endif
        /// <summary>The full path to the application configuration INI file.</summary>
        public static readonly string AppConfigIniPath = System.IO.Path.Combine(AppConfigDirectory, "config.ini");
        /// <summary>The directory for application modules or custom data.</summary>
        public static readonly string AppModuleDirectory = System.IO.Path.Combine(AppConfigDirectory, "modules");
        /// <summary>The directory used for caching extracted or generated icons.</summary>
        public static readonly string AppIconCacheDirectory = System.IO.Path.Combine(AppDataDirectory, "cache_icons");


        // --- Menu Names ---

        /// <summary>
        /// Contains constant strings for built-in radial menu names.
        /// </summary>
        public static class MenuNames
        {
            /// <summary>The name of the main, top-level menu.</summary>
            public static readonly string Root = "ROOT";
        }

        // --- Action Identifiers ---

        /// <summary>
        /// Contains constant strings for built-in action IDs.
        /// </summary>
        public static class BuiltInActionIds
        {
            public static readonly string Clockwise = "CLOCKWISE";
            public static readonly string Counterclockwise = "COUNTERCLOCKWISE";
            /// <summary>Null command (does nothing).</summary>
            public static readonly string Empty = "EMPTY";
            /// <summary>Opens main/root menu.</summary>
            public static readonly string Root = "ROOT";
            /// <summary>Closes the current menu.</summary>
            public static readonly string Close = "CLOSE";
            /// <summary>Display a 'Hello' message.</summary>
            public static readonly string Hello = "HELLO";
            /// <summary>Captures a screenshot or clip.</summary>
            public static readonly string ScreenClip = "SCREEN_CLIP";
            /// <summary>Opens the calculator application.</summary>
            public static readonly string Calculator = "CALCULATOR";

            /// <summary>Toggles media playback (Play/Pause).</summary>
            public static readonly string MediaPlayPause = "MEDIA_PLAY_PAUSE";
            /// <summary>Skips to the next media track.</summary>
            public static readonly string MediaNextTrack = "MEDIA_NEXT_TRACK";
            /// <summary>Returns to the previous media track.</summary>
            public static readonly string MediaPrevTrack = "MEDIA_PREV_TRACK";

            /// <summary>Increases system audio volume.</summary>
            public static readonly string AudioVolumeUp = "AUDIO_VOLUME_UP";
            /// <summary>Decreases system audio volume.</summary>
            public static readonly string AudioVolumeDown = "AUDIO_VOLUME_DOWN";
            /// <summary>Toggles system audio mute state.</summary>
            public static readonly string AudioVolumeMute = "AUDIO_VOLUME_MUTE";
        }

        // --- Action Scripts (Lua Syntax) ---

        /// <summary>
        /// Contains the Lua script snippets corresponding to built-in actions.
        /// </summary>
        public static class BuiltInActionScripts
        {
            public static readonly string Root = $"menu:open_menu('{MenuNames.Root}')";
            public static readonly string Close = "menu:close_menu()";
            public static readonly string Hello = "menu:print_message('Hello')";
            public static readonly string ScreenClip = "menu:close_menu()\nsys:open('ms-screenclip:')";
            public static readonly string Calculator = "menu:close_menu()\nsys:open('ms-calculator:')";

            public static readonly string MediaPlayPause = "menu:close_menu()\nmedia:play_or_pause_async()";
            public static readonly string MediaNextTrack = "menu:close_menu()\nmedia:skip_next_async()";
            public static readonly string MediaPrevTrack = "menu:close_menu()\nmedia:skip_previous_async()";

            public static readonly string AudioVolumeUp = "audio:increase_volume()";
            public static readonly string AudioVolumeDown = "audio:decrease_volume()";
            public static readonly string AudioVolumeMute = "--!call-next"; // Special marker for action execution logic
        }

        // --- Built-in Mappings ---

        /// <summary>A read-only dictionary mapping action IDs to their corresponding Lua scripts.</summary>
        public static readonly System.Collections.Generic.IReadOnlyDictionary<string, string> ActionScriptsMap;
        /// <summary>A read-only dictionary mapping action IDs to their built-in icon URIs.</summary>
        public static readonly System.Collections.Generic.IReadOnlyDictionary<string, string> BuiltInIconPaths;
        /// <summary>A read-only dictionary mapping action IDs to their pre-created MenuOptionData objects.</summary>
        public static readonly System.Collections.Generic.IReadOnlyDictionary<string, MenuOptionData> BuiltInOptionsMap;

        // --- Built-in Menu Options ---

        /// <summary>
        /// Contains pre-defined instances of <see cref="MenuOptionData"/> for built-in actions.
        /// </summary>
        public static class BuiltInOptions
        {
            public static readonly MenuOptionData Empty;
            public static readonly MenuOptionData Close;
            public static readonly MenuOptionData Hello;
            public static readonly MenuOptionData ScreenClip;
            public static readonly MenuOptionData Calculator;

            public static readonly MenuOptionData MediaPlayPause;
            public static readonly MenuOptionData MediaNextTrack;
            public static readonly MenuOptionData MediaPrevTrack;

            public static readonly MenuOptionData AudioVolumeUp;
            public static readonly MenuOptionData AudioVolumeDown;
            public static readonly MenuOptionData AudioVolumeMute;

            /// <summary>The default options array for the root menu.</summary>
            public static readonly MenuOptionData[] RootOptions;

            /// <summary>Initializes all built-in menu options.</summary>
            static BuiltInOptions()
            {
                Empty = new MenuOptionData
                {
                    Id = BuiltInActionIds.Empty,
                    InternalIconResourcePath = "",
                    ActionId = BuiltInActionIds.Empty
                };
                Close = new MenuOptionData
                {
                    Id = BuiltInActionIds.Close,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.Close],
                    ActionId = BuiltInActionIds.Close
                };
                Hello = new MenuOptionData
                {
                    Id = BuiltInActionIds.Hello,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.Hello],
                    ActionId = BuiltInActionIds.Hello
                };
                ScreenClip = new MenuOptionData
                {
                    Id = BuiltInActionIds.ScreenClip,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.ScreenClip],
                    ActionId = BuiltInActionIds.ScreenClip
                };
                Calculator = new MenuOptionData
                {
                    Id = BuiltInActionIds.Calculator,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.Calculator],
                    ActionId = BuiltInActionIds.Calculator
                };
                MediaPlayPause = new MenuOptionData
                {
                    Id = BuiltInActionIds.MediaPlayPause,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.MediaPlayPause],
                    ActionId = BuiltInActionIds.MediaPlayPause
                };
                MediaNextTrack = new MenuOptionData
                {
                    Id = BuiltInActionIds.MediaNextTrack,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.MediaNextTrack],
                    ActionId = BuiltInActionIds.MediaNextTrack
                };
                MediaPrevTrack = new MenuOptionData
                {
                    Id = BuiltInActionIds.MediaPrevTrack,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.MediaPrevTrack],
                    ActionId = BuiltInActionIds.MediaPrevTrack
                };
                AudioVolumeUp = new MenuOptionData
                {
                    Id = BuiltInActionIds.AudioVolumeUp,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.AudioVolumeUp],
                    ActionId = BuiltInActionIds.AudioVolumeUp
                };
                AudioVolumeDown = new MenuOptionData
                {
                    Id = BuiltInActionIds.AudioVolumeDown,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.AudioVolumeDown],
                    ActionId = BuiltInActionIds.AudioVolumeDown
                };
                AudioVolumeMute = new MenuOptionData
                {
                    Id = BuiltInActionIds.AudioVolumeMute,
                    InternalIconResourcePath = BuiltInIconPaths[BuiltInActionIds.AudioVolumeMute],
                    ActionId = BuiltInActionIds.AudioVolumeMute
                };

                RootOptions = [AudioVolumeUp, AudioVolumeMute, AudioVolumeDown];
            }
        }

        /// <summary>
        /// Static constructor to initialize built-in action, icon, and option maps.
        /// </summary>
        static AppConstants()
        {
            ActionScriptsMap = new System.Collections.Generic.Dictionary<string, string>
            {
                {BuiltInActionIds.Empty, "" },
                {BuiltInActionIds.Close, BuiltInActionScripts.Close },
                {BuiltInActionIds.Hello, BuiltInActionScripts.Hello },
                {BuiltInActionIds.ScreenClip, BuiltInActionScripts.ScreenClip },
                {BuiltInActionIds.Calculator, BuiltInActionScripts.Calculator },
                {BuiltInActionIds.MediaPlayPause, BuiltInActionScripts.MediaPlayPause },
                {BuiltInActionIds.MediaNextTrack, BuiltInActionScripts.MediaNextTrack },
                {BuiltInActionIds.MediaPrevTrack, BuiltInActionScripts.MediaPrevTrack },
                {BuiltInActionIds.AudioVolumeUp, BuiltInActionScripts.AudioVolumeUp },
                {BuiltInActionIds.AudioVolumeDown, BuiltInActionScripts.AudioVolumeDown },
                {BuiltInActionIds.AudioVolumeMute, BuiltInActionScripts.AudioVolumeMute },
            };

            BuiltInIconPaths = new System.Collections.Generic.Dictionary<string, string>
            {
                {BuiltInActionIds.Close, "pack://application:,,,/RotorisLib;component/Resources/option_icons/x-circle.png" },
                {BuiltInActionIds.Hello, "pack://application:,,,/RotorisLib;component/Resources/option_icons/hand-waving.png" },
                {BuiltInActionIds.ScreenClip, "pack://application:,,,/RotorisLib;component/Resources/option_icons/crop.png" },
                {BuiltInActionIds.Calculator, "pack://application:,,,/RotorisLib;component/Resources/option_icons/calculator.png" },
                {BuiltInActionIds.MediaPlayPause, "pack://application:,,,/RotorisLib;component/Resources/option_icons/play-pause.png" },
                {BuiltInActionIds.MediaNextTrack, "pack://application:,,,/RotorisLib;component/Resources/option_icons/skip-forward.png" },
                {BuiltInActionIds.MediaPrevTrack, "pack://application:,,,/RotorisLib;component/Resources/option_icons/skip-back.png" },
                {BuiltInActionIds.AudioVolumeUp, "pack://application:,,,/RotorisLib;component/Resources/option_icons/speaker-high.png" },
                {BuiltInActionIds.AudioVolumeDown, "pack://application:,,,/RotorisLib;component/Resources/option_icons/speaker-low.png" },
                {BuiltInActionIds.AudioVolumeMute, "pack://application:,,,/RotorisLib;component/Resources/option_icons/speaker-x.png"},
            };

            BuiltInOptionsMap = new System.Collections.Generic.Dictionary<string, MenuOptionData>
            {
                {BuiltInActionIds.Close, BuiltInOptions.Close },
                {BuiltInActionIds.Hello, BuiltInOptions.Hello },
                {BuiltInActionIds.ScreenClip, BuiltInOptions.ScreenClip },
                {BuiltInActionIds.Calculator, BuiltInOptions.Calculator },
                {BuiltInActionIds.MediaPlayPause, BuiltInOptions.MediaPlayPause },
                {BuiltInActionIds.MediaNextTrack, BuiltInOptions.MediaNextTrack },
                {BuiltInActionIds.MediaPrevTrack, BuiltInOptions.MediaPrevTrack },
                {BuiltInActionIds.AudioVolumeUp, BuiltInOptions.AudioVolumeUp },
                {BuiltInActionIds.AudioVolumeDown, BuiltInOptions.AudioVolumeDown },
                {BuiltInActionIds.AudioVolumeMute, BuiltInOptions.AudioVolumeMute },
            };
        }

        /// <summary>A singleton instance of the TextImageGenerator for creating icon images from text.</summary>
        public static readonly TextImageGenerator TextImageRenderer = new();

    }
}