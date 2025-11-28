namespace RotorisLib
{
    public class FileCollect
    {
        /// <summary>
        /// Recursively collects the **full paths** of all files within a specified directory and its subdirectories.
        /// It includes an internal filter to **exclude** files whose path contains a directory separator followed by a dot (e.g., hidden files or files within hidden directories like **.git**).
        /// </summary>
        /// <param name="baseDirectoryPath">The starting directory path to search from.</param>
        /// <param name="pattern">The search pattern to match against the names of files (e.g., "*.txt", "*.*"). Default is "*.*".</param>
        /// <returns>An array of full file paths, or an empty array if the directory does not exist or an access error occurs.</returns>
        public static string[] CollectRelativeFilePaths(string baseDirectoryPath, string pattern = "*.*")
        {
            if (!System.IO.Directory.Exists(baseDirectoryPath))
            {
                Console.WriteLine($"Error: The specified directory '{baseDirectoryPath}' does not exist.");
                return [];
            }

            string normalizedBasePath = baseDirectoryPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString())
                                    ? baseDirectoryPath
                                    : baseDirectoryPath + System.IO.Path.DirectorySeparatorChar;

            List<string> filePaths = [];

            try
            {
                foreach (string filePath in System.IO.Directory.EnumerateFiles(normalizedBasePath, pattern, System.IO.SearchOption.AllDirectories))
                {
                    if (filePath.Contains(System.IO.Path.DirectorySeparatorChar + ".", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    filePaths.Add(filePath.Replace(System.IO.Path.DirectorySeparatorChar, '/'));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access to the path is denied. Details: {ex.Message}");
                return [.. filePaths];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during file enumeration. Details: {ex.Message}");
                return [.. filePaths];
            }

            return [.. filePaths];
        }

        /// <summary>
        /// Converts an array of full file paths into a dictionary, where the key is the **full path** and the value is the **relative, cross-platform path**.
        /// The relative path is normalized to use forward slashes (/) as separators and is made relative to the base directory path.
        /// **Note:** This method assumes the input files array was generated using the same **baseDirectoryPath**.
        /// </summary>
        /// <param name="baseDirectoryPath">The base directory path used to make the file paths relative.</param>
        /// <param name="files">An array of full file paths.</param>
        /// <returns>A dictionary mapping the full path to the cross-platform relative path.</returns>
        public static Dictionary<string, string> NormalizeFiles(string baseDirectoryPath, string[] files)
        {
            string normalizedDirPath = baseDirectoryPath.TrimEnd(System.IO.Path.DirectorySeparatorChar) + System.IO.Path.DirectorySeparatorChar;
            Dictionary<string, string> normalizedFiles = [];
            foreach (string file in files)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                normalizedFiles[file] = file[normalizedDirPath.Length..].Replace(System.IO.Path.DirectorySeparatorChar, '/');
            }
            return normalizedFiles;
        }

        /// <summary>
        /// Creates a list of **relative, cross-platform file paths** from the provided dictionary, filtering by an optional file suffix.
        /// This is often used to get a simple list of paths (without full path keys) for a specific file type.
        /// </summary>
        /// <param name="files">A dictionary mapping full file paths (key) to relative, cross-platform file paths (value), typically from **NormalizeFiles**.</param>
        /// <param name="fileSuffix">The file suffix (e.g., ".md", ".json") to filter the files by. Default is an empty string (no suffix filter).</param>
        /// <returns>A list of relative, cross-platform file paths matching the suffix.</returns>
        public static List<string> CreateFileList(Dictionary<string, string> files, string fileSuffix = "")
        {
            List<string> fileList = [];
            foreach (var file in files)
            {
                if (!file.Key.EndsWith(fileSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                fileList.Add(file.Value);
            }

            return fileList;
        }

        /// <summary>
        /// Reads the content of files matching a specified suffix, processes the content using a provided function,
        /// and stores the results in a cache dictionary. It creates multiple keys for easy access:
        /// 1. The original **relative path with suffix** (e.g., "path/to/file.ext").
        /// 2. The **relative path without suffix** (e.g., "path/to/file").
        /// 3. The **directory path** if the file is named "index" (e.g., "path/to" for "path/to/index.ext").
        /// </summary>
        /// <typeparam name="T">The type of the processed value to be stored in the cache.</typeparam>
        /// <param name="files">A dictionary mapping full file paths (key) to relative, cross-platform file paths (value), typically from **NormalizeFiles**.</param>
        /// <param name="fileSuffix">The file suffix (e.g., ".md", ".json") to filter files.</param>
        /// <param name="processor">A function to process the file's text content and return a value of type T (or null if processing fails).</param>
        /// <returns>A dictionary containing the processed file content (value) mapped to various cross-platform keys (key).</returns>
        public static Dictionary<string, T> CreateFileCache<T>(Dictionary<string, string> files, string fileSuffix, Func<string, T?> processor)
        {
            Dictionary<string, T> cache = [];

            foreach (var file in files)
            {
                string filePath = file.Key;
                if (!filePath.EndsWith(fileSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    string fileContent = System.IO.File.ReadAllText(filePath);
                    T? processedValue = processor(fileContent);
                    if (processedValue == null)
                    {
                        continue;
                    }

                    string crossPlatformPath = file.Value;

                    cache[crossPlatformPath] = processedValue;

                    string keyWithoutSuffix = crossPlatformPath[..^fileSuffix.Length];
                    if (!cache.ContainsKey(keyWithoutSuffix))
                    {
                        cache[keyWithoutSuffix] = processedValue;
                        Console.WriteLine($"[FileCache] Mapping '{keyWithoutSuffix}' -> '{crossPlatformPath}'");
                    }

                    if (crossPlatformPath.EndsWith("/index" + fileSuffix, StringComparison.OrdinalIgnoreCase))
                    {
                        string directoryKey = crossPlatformPath.Substring(0, crossPlatformPath.Length - (6 + fileSuffix.Length));
                        cache[directoryKey] = processedValue;
                        Console.WriteLine($"[FileCache] Mapping '{directoryKey}' -> '{crossPlatformPath}'");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[FileCache] Error processing file '{filePath}': {ex.Message}");
                }
            }

            return cache;
        }
    }
}
