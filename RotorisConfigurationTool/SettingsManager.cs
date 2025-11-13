using System.Windows.Media;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using RotorisLib;

// Aliases for RotorisLib internal constants
using SectionColumns = RotorisLib.Configuration.SectionColumns;
using Sections = RotorisLib.Configuration.Sections;

namespace RotorisConfigurationTool
{
    /// <summary>
    /// Manages the application configuration, including INI settings, radial menus, and Lua actions.
    /// </summary>
    public class SettingsManager
    {
        /// <summary>
        /// Represents the metadata parsed from the header comments of a Lua action file.
        /// </summary>
        public class ScriptMetadata
        {
            /// <summary>
            /// Gets or sets a value indicating whether the script execution should continue to the next handler.
            /// Corresponds to the '---!callnext' directive.
            /// </summary>
            public required bool CallNext { set; get; }

            /// <summary>
            /// Gets or sets the command string associated with the action.
            /// Corresponds to the '---!commands' directive.
            /// </summary>
            public required string Commands { set; get; }
        }

        public static readonly CultureInfo CurrentCulture = Thread.CurrentThread.CurrentUICulture;
        /// <summary>
        /// The normalized file name for the application's root radial menu configuration.
        /// </summary>
        public static readonly string RootRadialMenuFileName = $"_{AppConstants.MenuNames.Root}.json";

        private readonly IniFile applicationIniFile = new(AppConstants.AppConfigIniPath);
        private Configuration currentConfig;

        // Caches for loaded menu and action files
        private readonly HashSet<string> radialMenuNames;
        private readonly Dictionary<string, MenuOptionData[]> radialMenus;
        private readonly HashSet<string> scriptFileNames;
        private readonly Dictionary<string, ScriptMetadata> scriptMetadataCache = [];

        /// <summary>
        /// Initializes a new instance of the ConfigurationManager class, loading settings and modules.
        /// </summary>
        public SettingsManager()
        {
            // 1. Load configuration from INI file
            currentConfig = new Configuration(applicationIniFile);

            // 2. Collect all module files (.json and .lua)
            string[] rawFiles = FileCollect.CollectRelativeFilePaths(AppConstants.AppModuleDirectory, "*.*") ?? [];
            Dictionary<string, string> normalizedFiles = FileCollect.NormalizeFiles(AppConstants.AppModuleDirectory, rawFiles);

            // 3. Process radial menus (.json)
            radialMenuNames = [.. FileCollect.CreateFileList(normalizedFiles, ".json")];
            radialMenus = FileCollect.CreateFileCache(normalizedFiles, ".json", MenuOptionData.ParseOptionsFromJson);

            // 4. Process Lua actions (.lua)
            scriptFileNames = [.. FileCollect.CreateFileList(normalizedFiles, ".lua")];
            foreach (var file in normalizedFiles)
            {
                if (file.Key.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
                {
                    if (ReadScriptMetadata(file.Key) is ScriptMetadata moduleMetadata)
                    {
                        scriptMetadataCache.Add(file.Value, moduleMetadata);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the metadata (header comments) from a Lua script file.
        /// </summary>
        /// <param name="filePath">The full path to the Lua file.</param>
        /// <returns>A ScriptMetadata object if the file is successfully read, otherwise null.</returns>
        private static ScriptMetadata? ReadScriptMetadata(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"ERROR: Script file not found at path: {filePath}");
                return null;
            }

            try
            {
                var lines = File.ReadLines(filePath);
                ScriptMetadata metadata = new()
                {
                    CallNext = false,
                    Commands = ""
                };

                int lineCount = 0;
                // Only read the first few lines to find metadata
                foreach (var line in lines)
                {
                    if (line.StartsWith("---!call-next", StringComparison.OrdinalIgnoreCase))
                    {
                        metadata.CallNext = true;
                    }
                    else if (line.StartsWith("---!commands ", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract command string, starting after "---!commands " (13 characters)
                        metadata.Commands = line[13..].Trim();
                    }

                    lineCount++;
                    // Stop reading after 5 lines to optimize file access
                    if (lineCount >= 5)
                    {
                        break;
                    }
                }

                return metadata;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Failed to read script metadata from '{filePath}'. Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the currently loaded application configuration settings.
        /// </summary>
        public Configuration CurrentConfig
        {
            get { return currentConfig; }
        }

        /// <summary>
        /// Updates the UI size setting.
        /// </summary>
        /// <param name="size">The new UI size value.</param>
        public void UpdateUiSize(double size)
        {
            currentConfig.UiSize = size;
        }

        /// <summary>
        /// Updates the UI color settings (Background, Foreground, Accent) from a new configuration object.
        /// </summary>
        /// <param name="newConfig">The configuration object containing new color values.</param>
        public void UpdateUiColors(Configuration newConfig)
        {
            currentConfig.UiBackground = newConfig.UiBackground;
            currentConfig.UiForeground = newConfig.UiForeground;
            currentConfig.UiAccent = newConfig.UiAccent;
        }

        /// <summary>
        /// Updates the key binding settings from a new configuration object.
        /// </summary>
        /// <param name="newConfig">The configuration object containing new key binding values.</param>
        public void UpdateKeyBindings(Configuration newConfig)
        {
            currentConfig.PrimaryKey = newConfig.PrimaryKey;
            currentConfig.ClockwiseKey = newConfig.ClockwiseKey;
            currentConfig.CounterclockwiseKey = newConfig.CounterclockwiseKey;
        }

        /// <summary>
        /// Saves the current configuration settings to the INI file.
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                // Appearance Section
                applicationIniFile.WriteValue(Sections.Appearance, SectionColumns.Appearance.UiSize, currentConfig.UiSize.ToString(CultureInfo.InvariantCulture));

                // Colors are saved as hex strings without the '#' prefix
                if (currentConfig.UiBackground is Color backgroundColor)
                {
                    applicationIniFile.WriteValue(Sections.Appearance, SectionColumns.Appearance.BackgroundColor, backgroundColor.ToString().TrimStart('#'));
                }
                else
                {
                    applicationIniFile.RemoveValue(Sections.Appearance, SectionColumns.Appearance.BackgroundColor);
                }

                if (currentConfig.UiForeground is Color foregroundColor)
                {
                    applicationIniFile.WriteValue(Sections.Appearance, SectionColumns.Appearance.ForegroundColor, foregroundColor.ToString().TrimStart('#'));
                }
                else
                {
                    applicationIniFile.RemoveValue(Sections.Appearance, SectionColumns.Appearance.ForegroundColor);
                }

                if (currentConfig.UiAccent is Color accentColor)
                {
                    applicationIniFile.WriteValue(Sections.Appearance, SectionColumns.Appearance.AccentColor, accentColor.ToString().TrimStart('#'));
                }
                else
                {
                    applicationIniFile.RemoveValue(Sections.Appearance, SectionColumns.Appearance.AccentColor);
                }

                // KeyBindings Section
                applicationIniFile.WriteValue(Sections.KeyBindings, SectionColumns.KeyBindings.PrimaryKey, currentConfig.PrimaryKey.ToString());
                applicationIniFile.WriteValue(Sections.KeyBindings, SectionColumns.KeyBindings.ClockwiseKey, currentConfig.ClockwiseKey.ToString());
                applicationIniFile.WriteValue(Sections.KeyBindings, SectionColumns.KeyBindings.CounterclockwiseKey, currentConfig.CounterclockwiseKey.ToString());

                applicationIniFile.Save();
                Debug.WriteLine("INFO: Application settings successfully saved to INI file.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Failed to save settings to INI file: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the dictionary containing all loaded radial menus (rings).
        /// </summary>
        public Dictionary<string, MenuOptionData[]> RadialMenus
        {
            get => radialMenus;
        }

        /// <summary>
        /// Gets an array of the normalized file names for all loaded radial menus.
        /// </summary>
        public string[] RadialMenuNames
        {
            get => [.. radialMenuNames];
        }

        /// <summary>
        /// Retrieves the options for a specific radial menu by its normalized file name.
        /// </summary>
        /// <param name="name">The normalized file name (e.g., "ring_name.json").</param>
        /// <returns>The array of menu options, or an array containing the AppConstants.BuiltInOptions.Empty if not found.</returns>
        public MenuOptionData[] GetMenuOptions(string name)
        {
            if (radialMenus.TryGetValue(name, out var options))
            {
                return options;
            }
            Debug.WriteLine($"WARNING: Radial menu '{name}' not found. Returning empty option.");
            return [AppConstants.BuiltInOptions.Empty];
        }

        /// <summary>
        /// Removes a radial menu configuration file and deletes it from disk.
        /// </summary>
        /// <param name="name">The normalized file name of the ring (with or without .json).</param>
        /// <returns>True if the file was deleted successfully, false otherwise.</returns>
        public bool RemoveRadialMenu(string name)
        {
            string normalizedName = name.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? name : name + ".json";

            if (!radialMenus.ContainsKey(normalizedName))
            {
                Debug.WriteLine($"WARNING: Attempted to remove non-existent menu: {normalizedName}");
                return false;
            }

            try
            {
                radialMenus.Remove(normalizedName);
                radialMenuNames.Remove(normalizedName);
                string filePath = GetFilePathForModule(normalizedName);
                File.Delete(filePath);
                Debug.WriteLine($"INFO: Radial menu '{normalizedName}' and its file deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Failed to delete radial menu file '{normalizedName}'. Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Renames a radial menu file on disk and updates the in-memory cache.
        /// </summary>
        /// <param name="oldName">The old normalized file name.</param>
        /// <param name="newName">The new desired file name (without extension).</param>
        /// <returns>True if the rename was successful, false otherwise.</returns>
        public bool RenameRadialMenu(string oldName, string newName)
        {
            if (!IsValidModuleName(newName))
            {
                Debug.WriteLine($"ERROR: Rename failed. New name '{newName}' contains invalid characters or reserved words.");
                return false;
            }

            string oldNormalizedName = oldName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? oldName : oldName + ".json";
            string newNormalizedName = newName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? newName : newName + ".json";

            if (radialMenus.ContainsKey(newNormalizedName))
            {
                Debug.WriteLine($"ERROR: Rename failed. A menu with the name '{newNormalizedName}' already exists.");
                return false;
            }

            if (radialMenus.TryGetValue(oldNormalizedName, out var options))
            {
                try
                {
                    radialMenus.Remove(oldNormalizedName);
                    radialMenus.Add(newNormalizedName, options);
                    radialMenuNames.Remove(oldNormalizedName);
                    radialMenuNames.Add(newNormalizedName);

                    string oldPath = GetFilePathForModule(oldNormalizedName);
                    string newPath = GetFilePathForModule(newNormalizedName);
                    File.Move(oldPath, newPath);
                    Debug.WriteLine($"INFO: Radial menu '{oldNormalizedName}' renamed to '{newNormalizedName}' successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR: Failed to rename radial menu file '{oldNormalizedName}' to '{newNormalizedName}'. Exception: {ex.Message}");
                    // Attempt to revert in-memory changes if file operation failed
                    radialMenus.Remove(newNormalizedName);
                    radialMenus.Add(oldNormalizedName, options);
                    radialMenuNames.Remove(newNormalizedName);
                    radialMenuNames.Add(oldNormalizedName);
                    return false;
                }
            }
            Debug.WriteLine($"ERROR: Rename failed. Original menu '{oldNormalizedName}' not found in cache.");
            return false;
        }

        /// <summary>
        /// Updates the options for a radial menu, saves the change to disk, and updates the in-memory cache.
        /// If the menu does not exist, it is created.
        /// </summary>
        /// <param name="name">The file name of the ring (with or without .json).</param>
        /// <param name="options">The new array of menu options.</param>
        /// <returns>True if the update/creation was successful, false otherwise.</returns>
        public bool UpdateRadialMenu(string name, MenuOptionData[] options)
        {
            if (!IsValidModuleName(name))
            {
                Debug.WriteLine($"ERROR: Update failed. Menu name '{name}' contains invalid characters or reserved words.");
                return false;
            }

            string normalizedName = name.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? name : name + ".json";

            try
            {
                radialMenus[normalizedName] = options;
                radialMenuNames.Add(normalizedName);

                string filePath = GetFilePathForModule(normalizedName);
                string ringJsonString = JsonConvert.SerializeObject(options, Newtonsoft.Json.Formatting.Indented);
                if (Path.GetDirectoryName(filePath) is string directory)
                {
                    Directory.CreateDirectory(directory);
                    File.WriteAllText(filePath, ringJsonString);
                    Debug.WriteLine($"INFO: Radial menu '{normalizedName}' updated and saved successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Failed to update/save radial menu file '{normalizedName}'. Exception: {ex.Message}");
            }

            return false;
        }

        // --- Lua Action Methods ---

        /// <summary>
        /// Gets the dictionary containing metadata for all external Lua actions.
        /// </summary>
        public Dictionary<string, ScriptMetadata> ExternalScriptMetadata
        {
            get => scriptMetadataCache;
        }

        /// <summary>
        /// Gets an array of the names of built-in actions (from AppConstants).
        /// </summary>
        public string[] BuiltInActionNames
        {
            get => [.. AppConstants.ActionScriptsMap.Keys];
        }

        /// <summary>
        /// Gets an array of the normalized file names for all external Lua action scripts.
        /// </summary>
        public string[] ExternalActionNames
        {
            get => [.. scriptFileNames];
        }

        /// <summary>
        /// Gets an array of all available action names (built-in and external).
        /// </summary>
        public string[] AvailableActionNames
        {
            get => [.. AppConstants.ActionScriptsMap.Keys, .. scriptFileNames];
        }

        /// <summary>
        /// Removes an external Lua action script file from disk and cache.
        /// </summary>
        /// <param name="name">The file name of the action (with or without .lua).</param>
        /// <returns>True if the file was deleted successfully, false otherwise.</returns>
        public bool RemoveExternalAction(string name)
        {
            string normalizedName = name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ? name : name + ".lua";

            if (!scriptMetadataCache.ContainsKey(normalizedName))
            {
                Debug.WriteLine($"WARNING: Attempted to remove non-existent external action: {normalizedName}");
                return false;
            }

            try
            {
                scriptMetadataCache.Remove(normalizedName);
                scriptFileNames.Remove(normalizedName);
                string filePath = GetFilePathForModule(normalizedName);
                File.Delete(filePath);
                Debug.WriteLine($"INFO: External action '{normalizedName}' file deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Failed to delete external action file '{normalizedName}'. Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Renames an external Lua action script file on disk and updates the in-memory cache.
        /// </summary>
        /// <param name="oldName">The old normalized file name.</param>
        /// <param name="newName">The new desired file name (without extension).</param>
        /// <returns>True if the rename was successful, false otherwise.</returns>
        public bool RenameExternalAction(string oldName, string newName)
        {
            if (!IsValidModuleName(newName))
            {
                Debug.WriteLine($"ERROR: Rename failed. New name '{newName}' contains invalid characters or reserved words.");
                return false;
            }

            string oldNormalizedName = oldName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ? oldName : oldName + ".lua";
            string newNormalizedName = newName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ? newName : newName + ".lua";

            if (scriptFileNames.Contains(newNormalizedName))
            {
                Debug.WriteLine($"ERROR: Rename failed. An action with the name '{newNormalizedName}' already exists.");
                return false;
            }

            if (scriptMetadataCache.TryGetValue(oldNormalizedName, out var metadata))
            {
                try
                {
                    scriptMetadataCache.Remove(oldNormalizedName);
                    scriptMetadataCache.Add(newNormalizedName, metadata);
                    scriptFileNames.Remove(oldNormalizedName);
                    scriptFileNames.Add(newNormalizedName);

                    string oldPath = GetFilePathForModule(oldNormalizedName);
                    string newPath = GetFilePathForModule(newNormalizedName);
                    File.Move(oldPath, newPath);
                    Debug.WriteLine($"INFO: External action '{oldNormalizedName}' renamed to '{newNormalizedName}' successfully.");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR: Failed to rename external action file '{oldNormalizedName}' to '{newNormalizedName}'. Exception: {ex.Message}");
                    // Attempt to revert in-memory changes if file operation failed
                    scriptMetadataCache.Remove(newNormalizedName);
                    scriptMetadataCache.Add(oldNormalizedName, metadata);
                    scriptFileNames.Remove(newNormalizedName);
                    scriptFileNames.Add(oldNormalizedName);
                    return false;
                }
            }
            Debug.WriteLine($"ERROR: Rename failed. Original action '{oldNormalizedName}' not found in cache.");
            return false;
        }

        /// <summary>
        /// Updates the metadata and content of an external Lua action script file.
        /// If the action does not exist, it is created.
        /// </summary>
        /// <param name="name">The file name of the action (with or without .lua).</param>
        /// <param name="callNext">Whether to include the '---!callnext' directive.</param>
        /// <param name="commands">The command string to include in the '---!commands' directive.</param>
        /// <returns>True if the update/creation was successful, false otherwise.</returns>
        public bool UpdateExternalAction(string name, bool callNext, string commands)
        {
            if (!IsValidModuleName(name))
            {
                Debug.WriteLine($"ERROR: Update failed. Action name '{name}' contains invalid characters or reserved words.");
                return false;
            }

            string normalizedName = name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ? name : name + ".lua";

            ScriptMetadata newMetadata = new()
            {
                CallNext = callNext,
                Commands = commands.Trim()
            };

            try
            {
                scriptMetadataCache[normalizedName] = newMetadata;
                scriptFileNames.Add(normalizedName);

                string filePath = GetFilePathForModule(normalizedName);

                // Write content based on metadata directives
                using var writer = new StreamWriter(filePath);

                // Write directives at the top of the file
                if (newMetadata.CallNext)
                {
                    writer.WriteLine("---!callnext");
                }
                if (!string.IsNullOrEmpty(newMetadata.Commands))
                {
                    writer.WriteLine($"---!commands {newMetadata.Commands}");
                }

                // Add a placeholder for actual Lua code, to prevent an empty file
                writer.WriteLine("\n-- Add your Lua code below this line.");

                Debug.WriteLine($"INFO: External action '{normalizedName}' updated and saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Failed to update/save external action file '{normalizedName}'. Exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a module name is valid for use as a filename/directory name.
        /// </summary>
        /// <param name="name">The proposed file or directory name.</param>
        /// <returns>True if the name is valid, false otherwise.</returns>
        public static bool IsValidModuleName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.WriteLine("VALIDATION ERROR: Module name cannot be empty.");
                return false;
            }
            // Invalid characters for file names, excluding path separators ('/') which might be used for normalized paths
            char[] invalidChars = [
                .. Path.GetInvalidFileNameChars().Where(
                    c => c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar
                    )
                ];

            if (name.IndexOfAny(invalidChars) >= 0)
            {
                Debug.WriteLine($"VALIDATION ERROR: Module name '{name}' contains invalid characters.");
                return false;
            }

            // Check for reserved words/substrings (case-insensitive check)
            if (name.Contains("DEBUG", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("VALIDATION ERROR: Module name contains the reserved word 'DEBUG'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the absolute file path for a module given its normalized file name.
        /// </summary>
        /// <param name="normalizedName">The normalized file name (e.g., "folder/file.ext").</param>
        /// <returns>The full file path.</returns>
        private static string GetFilePathForModule(string normalizedName)
        {
            // Replace normalized path separators ('/') with system-specific ones
            string relativePath = normalizedName.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(AppConstants.AppModuleDirectory, relativePath);
        }

        /// <summary>
        /// Retrieves the configured or system default color for a specific theme element.
        /// </summary>
        /// <param name="config">The current application configuration.</param>
        /// <param name="identifier">The identifier for the theme brush (Background, Foreground, or Accent).</param>
        /// <returns>The determined color, or null if the identifier is unknown.</returns>
        public static Color? GetColorForThemeIdentifier(Configuration config, RotorisLib.UserInterface.ThemeBrushIdentifier identifier)
        {
            // Use the renamed classes and enums from RotorisLib.UserInterface
            return identifier switch
            {
                UserInterface.ThemeBrushIdentifier.Accent => config.UiAccent ?? UserInterface.AppThemeBrushes.SystemAccentColor,
                UserInterface.ThemeBrushIdentifier.Foreground => config.UiForeground ?? UserInterface.AppThemeBrushes.SystemForegroundColor,
                UserInterface.ThemeBrushIdentifier.Background => config.UiBackground ?? UserInterface.AppThemeBrushes.SystemBackgroundColor,
                _ => null,
            };
        }

        public static string RemoveJsonSuffix(string value)
        {
            return value.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? value[..^5] : value;
        }
    }
}