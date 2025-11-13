namespace RotorisLib
{
    /// <summary>
    /// Represents a utility class for handling INI file operations, including parsing
    /// and persistence. Keys and section names are handled case-insensitively.
    /// </summary>
    public class IniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> sections = new(StringComparer.OrdinalIgnoreCase);
        private readonly string filePath;
        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class and loads
        /// the content from the specified INI file path.
        /// </summary>
        /// <param name="path">The full path to the INI file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
        public IniFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            filePath = path;

            if (!System.IO.File.Exists(path))
            {
                return;
            }

            string currentSection = "";
            try
            {
                foreach (string line in System.IO.File.ReadLines(path))
                {
                    string annotationRemoved = RemoveAnnotation(line);
                    if (IsSection(annotationRemoved, out string section))
                    {
                        if (!sections.ContainsKey(section))
                        {
                            sections.Add(section, new(StringComparer.OrdinalIgnoreCase));
                        }
                        currentSection = section;
                    }
                    else if (!string.IsNullOrEmpty(currentSection) && IsKeyValue(annotationRemoved, out string key, out string value))
                    {
                        sections[currentSection][key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: An unexpected error occurred while parsing INI file '{filePath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Reads a value from the specified section and key.
        /// </summary>
        /// <param name="sectionName">The name of the section (case-insensitive).</param>
        /// <param name="key">The key name (case-insensitive).</param>
        /// <param name="defaultValue">The value to return if the section or key is not found. Defaults to an empty string.</param>
        /// <returns>The string value associated with the key, or <paramref name="defaultValue"/> if the key/section is not found.</returns>
        public string ReadValue(string sectionName, string key, string defaultValue = "")
        {
            string upperSectionName = sectionName.ToUpperInvariant();
            string upperKey = key.ToUpperInvariant();

            if (sections.TryGetValue(upperSectionName, out var section))
            {
                if (section.TryGetValue(upperKey, out string? value))
                {
                    return value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Attempts to read a value from the specified section and key.
        /// </summary>
        /// <param name="sectionName">The name of the section (case-insensitive).</param>
        /// <param name="key">The key name (case-insensitive).</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter (an empty string).</param>
        /// <returns><c>true</c> if the section and key were found and the value was retrieved; otherwise, <c>false</c>.</returns>
        public bool ReadValue(string sectionName, string key, out string value)
        {
            string upperSectionName = sectionName.ToUpperInvariant();
            string upperKey = key.ToUpperInvariant();

            if (sections.TryGetValue(upperSectionName, out var section))
            {
                if (section.TryGetValue(upperKey, out string? v))
                {
                    value = v ?? "";
                    return true;
                }
            }
            value = "";
            return false;
        }

        /// <summary>
        /// Writes or updates a value for the specified section and key in memory.
        /// </summary>
        /// <remarks>
        /// This method only modifies the internal state; <see cref="Save"/> must be called
        /// to persist the changes to the file.
        /// </remarks>
        /// <param name="sectionName">The name of the section (case-insensitive).</param>
        /// <param name="key">The key name (case-insensitive).</param>
        /// <param name="value">The new string value to be written.</param>
        public void WriteValue(string sectionName, string key, string value)
        {
            string upperSectionName = sectionName.ToUpperInvariant();
            string upperKey = key.ToUpperInvariant();

            if (sections.TryGetValue(upperSectionName, out var section))
            {
                section[upperKey] = value;
            }
            else
            {
                Dictionary<string, string> newSection = new(StringComparer.OrdinalIgnoreCase);
                sections[upperSectionName] = newSection;
                newSection.Add(upperKey, value);
            }
        }

        public bool RemoveValue(string sectionName, string key)
        {
            string upperSectionName = sectionName.ToUpperInvariant();
            string upperKey = key.ToUpperInvariant();

            if (sections.TryGetValue(upperSectionName, out var section))
            {
                return section.Remove(upperKey);
            }
            return false;
        }
        /// <summary>
        /// Writes the in-memory INI data back to the file specified during initialization.
        /// </summary>
        /// <remarks>
        /// This operation overwrites the existing file content with the current in-memory state.
        /// </remarks>
        public void Save()
        {
            try
            {
                using System.IO.FileStream fs = System.IO.File.Open(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                using System.IO.StreamWriter sw = new(fs);

                foreach (var section in sections)
                {
                    sw.WriteLine($"[{section.Key}]");

                    foreach (var keyValueEntry in section.Value)
                    {
                        sw.WriteLine($"{keyValueEntry.Key}={keyValueEntry.Value}");
                    }
                    sw.WriteLine();
                }
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine($"ERROR: Failed to save INI file to '{filePath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Removes the comment (starting with '#') from a line of INI content.
        /// </summary>
        /// <param name="str">The INI line to process.</param>
        /// <returns>The trimmed string content before the first '#' character, or the original trimmed string if no '#' is found.</returns>
        private static string RemoveAnnotation(string str)
        {
            int hashIndex = str.IndexOf('#');
            if (hashIndex > -1)
            {
                return str[..hashIndex].Trim();
            }
            return str.Trim();
        }

        /// <summary>
        /// Checks if a line represents an INI section header (e.g., [SectionName]).
        /// </summary>
        /// <param name="str">The trimmed line content without comments.</param>
        /// <param name="section">When this method returns, contains the extracted and standardized (upper-invariant) section name if the line is a section; otherwise, an empty string.</param>
        /// <returns><c>true</c> if the line is a valid section header; otherwise, <c>false</c>.</returns>
        private static bool IsSection(string str, out string section)
        {
            int leftBracketIndex = str.IndexOf('[');
            if (leftBracketIndex == 0)
            {
                int rightBracketIndex = str.IndexOf(']');
                if (rightBracketIndex > -1)
                {
                    section = str[1..rightBracketIndex].Trim().ToUpperInvariant();
                    return true;
                }
            }

            section = "";
            return false;
        }

        /// <summary>
        /// Checks if a line represents an INI key-value pair (e.g., Key=Value).
        /// </summary>
        /// <param name="str">The trimmed line content without comments.</param>
        /// <param name="key">When this method returns, contains the extracted and standardized (upper-invariant) key name if the line is a key-value pair; otherwise, an empty string.</param>
        /// <param name="value">When this method returns, contains the extracted value if the line is a key-value pair; otherwise, an empty string.</param>
        /// <returns><c>true</c> if the line is a valid key-value pair; otherwise, <c>false</c>.</returns>
        private static bool IsKeyValue(string str, out string key, out string value)
        {
            int equalIndex = str.IndexOf('=');
            if (equalIndex > -1)
            {
                key = str[..equalIndex].Trim().ToUpperInvariant();
                value = str[(equalIndex + 1)..].Trim();
                return true;
            }
            key = "";
            value = "";
            return false;
        }
    }
}
