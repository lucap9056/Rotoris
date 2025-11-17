namespace RotorisLib
{
    // --- Menu Data Structures ---

    /// <summary>
    /// Represents the data for a single radial menu option.
    /// </summary>
    public struct MenuOptionData
    {
        [Newtonsoft.Json.JsonIgnore]
        public string Hash { get; set; }
        /// <summary>A unique identifier for the option.</summary>
        public string Id { get; set; }
        /// <summary>The path or URI to the icon image.</summary>
        public string IconPath { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string InternalIconResourcePath { get; set; }
        /// <summary>The ID of the action script to execute when selected.</summary>
        public string ActionId { get; set; }

        public void ResolveIconPath()
        {
            InternalIconResourcePath = ResolveIconPathAndCache(this);
        }

        /// <summary>
        /// Computes the SHA256 hash of an input string and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="input">The string to hash.</param>
        /// <returns>The SHA256 hash as a lowercase hexadecimal string.</returns>
        private static string ComputeSHA256Hash(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Use System.Security.Cryptography.SHA256 for hashing
            byte[] hashBytes = System.Security.Cryptography.SHA256.HashData(inputBytes); // Use ComputeHash for non-static access

            System.Text.StringBuilder sb = new();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();

        }

        /// <summary>
        /// Deserializes a JSON string into an array of <see cref="MenuOptionData"/> and resolves built-in actions/icons.
        /// </summary>
        /// <param name="json">The JSON string containing the menu options array.</param>
        /// <returns>An array of resolved <see cref="MenuOptionData"/>.</returns>
        /// <exception cref="Newtonsoft.Json.JsonSerializationException">Thrown if deserialization fails.</exception>
        public static MenuOptionData[] ParseOptionsFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return [];
            }

            try
            {
                var options = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuOptionData[]>(json);
                if (options == null)
                {
                    System.Diagnostics.Debug.WriteLine("LOG: JSON deserialization yielded null. Returning empty array.");
                    return [];
                }

                for (int i = 0; i < options.Length; i++)
                {
                    MenuOptionData currentOption = options[i];

                    if (AppConstants.BuiltInOptionsMap.TryGetValue(currentOption.Id, out MenuOptionData builtInOption))
                    {
                        options[i] = builtInOption;
                        continue;
                    }

                    options[i].InternalIconResourcePath = ResolveIconPathAndCache(options[i]);

                    if (AppConstants.ActionScriptsMap.TryGetValue(currentOption.ActionId, out string? script))
                    {
                        options[i].ActionId = script;
                    }

                }
                return options;
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                string errorMessage = $"Failed to parse menu options from JSON. Check the JSON string format. Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ERROR: {errorMessage}");
                throw new Newtonsoft.Json.JsonSerializationException(errorMessage, ex);
            }
            catch (System.Exception ex)
            {
                string errorMessage = $"An unexpected error occurred while parsing and resolving menu options. Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"FATAL: {errorMessage}");
                throw new System.Exception(errorMessage, ex);
            }
        }

        public static string ResolveIconPathAndCache(MenuOptionData option)
        {
            if (!string.IsNullOrEmpty(option.Id) && AppConstants.BuiltInOptionsMap.TryGetValue(option.Id, out MenuOptionData builtInOption))
            {
                return builtInOption.IconPath;
            }

            if (!string.IsNullOrEmpty(option.IconPath) && AppConstants.BuiltInIconPaths.TryGetValue(option.IconPath, out string? builtInIconPath))
            {
                return builtInIconPath;
            }

            if (System.IO.File.Exists(option.IconPath))
            {
                if (option.IconPath.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
                {
                    string iconName = ComputeSHA256Hash(option.IconPath) + ".ico";
                    string iconPath = System.IO.Path.Combine(AppConstants.AppIconCacheDirectory, iconName);

                    System.IO.Directory.CreateDirectory(AppConstants.AppIconCacheDirectory);

                    if (!System.IO.File.Exists(iconPath))
                    {
                        try
                        {
                            using System.Drawing.Icon? appIcon = System.Drawing.Icon.ExtractAssociatedIcon(option.IconPath);
                            using var fs = new System.IO.FileStream(iconPath, System.IO.FileMode.Create);
                            appIcon?.Save(fs);

                            System.Diagnostics.Debug.WriteLine($"LOG: Extracted icon from '{option.IconPath}' to cache path: '{iconPath}'.");
                            return iconPath;
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"ERROR: Failed to extract icon from '{option.IconPath}'. Error: {ex.Message}");
                        }
                    }
                }
                else
                {
                    return option.IconPath;
                }
            }
            else if (!string.IsNullOrEmpty(option.Id) && string.IsNullOrEmpty(option.IconPath))
            {
                string iconName = ComputeSHA256Hash(option.Id + "_id_text") + ".png";
                string iconPath = System.IO.Path.Combine(AppConstants.AppIconCacheDirectory, iconName);

                System.IO.Directory.CreateDirectory(AppConstants.AppIconCacheDirectory);

                if (!System.IO.File.Exists(iconPath))
                {
                    System.Diagnostics.Debug.WriteLine($"LOG: Generated text icon for ID '{option.Id}' at path: '{iconPath}'.");
                }
                return iconPath;
            }

            return option.IconPath;
        }


    }

}
