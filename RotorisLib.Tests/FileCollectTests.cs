using Xunit;
using RotorisLib;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace RotorisLib.Tests
{
    public class FileCollectTests : IDisposable
    {
        private string _tempTestDirectory;
        private StringWriter _consoleOutput;

        public FileCollectTests()
        {
            _tempTestDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempTestDirectory);

            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
            Console.SetError(_consoleOutput);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempTestDirectory))
            {
                Directory.Delete(_tempTestDirectory, true);
            }
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            Console.SetError(new StreamWriter(Console.OpenStandardError()));
            _consoleOutput.Dispose();
        }

        private string FilePath(string subPath)
        {
            return Path.Combine(_tempTestDirectory, subPath).Replace(Path.DirectorySeparatorChar, '/');
        }
        private string CreateFile(string subPath, string content = "")
        {
            string fullPath = Path.Combine(_tempTestDirectory, subPath);
            string? directoryName = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.WriteAllText(fullPath, content);
            return fullPath;
        }

        [Fact]
        public void CollectRelativeFilePaths_DirectoryDoesNotExist_ReturnsEmptyArrayAndLogsError()
        {
            string nonExistentPath = Path.Combine(_tempTestDirectory, "non_existent_dir");
            var files = FileCollect.CollectRelativeFilePaths(nonExistentPath);

            Assert.Empty(files);
            Assert.Contains($"Error: The specified directory '{nonExistentPath}' does not exist.", _consoleOutput.ToString());
        }

        [Fact]
        public void CollectRelativeFilePaths_EmptyDirectory_ReturnsEmptyArray()
        {
            var files = FileCollect.CollectRelativeFilePaths(_tempTestDirectory);

            Assert.Empty(files);
        }

        [Fact]
        public void CollectRelativeFilePaths_FlatDirectoryMatchingPattern_ReturnsCorrectFiles()
        {
            CreateFile("file1.txt");
            CreateFile("file2.lua");
            CreateFile("another.txt");

            var files = FileCollect.CollectRelativeFilePaths(_tempTestDirectory, "*.txt");

            Assert.Equal(2, files.Length);
            Assert.Contains(FilePath("file1.txt"), files);
            Assert.Contains(FilePath("another.txt"), files);
            Assert.DoesNotContain(FilePath("file2.lua"), files);
        }

        [Fact]
        public void CollectRelativeFilePaths_FlatDirectoryAllFiles_ReturnsAllFiles()
        {
            CreateFile("file1.txt");
            CreateFile("file2.lua");

            var files = FileCollect.CollectRelativeFilePaths(_tempTestDirectory); // Default pattern *.*

            Assert.Equal(2, files.Length);
            Assert.Contains(FilePath("file1.txt"), files);
            Assert.Contains(FilePath("file2.lua"), files);
        }

        [Fact]
        public void CollectRelativeFilePaths_NestedDirectoryMatchingPattern_ReturnsCorrectFiles()
        {
            CreateFile("sub/file1.txt");
            CreateFile("sub/sub2/file2.lua");
            CreateFile("sub/sub2/another.txt");
            CreateFile("root.lua");

            var files = FileCollect.CollectRelativeFilePaths(_tempTestDirectory, "*.txt");

            Assert.Equal(2, files.Length);
            Assert.Contains(FilePath("sub/file1.txt"), files);
            Assert.Contains(FilePath("sub/sub2/another.txt"), files);
            Assert.DoesNotContain(FilePath("root.lua"), files);
            Assert.DoesNotContain(FilePath("sub/sub2/file2.lua"), files);
        }

        [Fact]
        public void CollectRelativeFilePaths_ExcludesHiddenFilesAndDirectories()
        {
            CreateFile("visible.txt");
            CreateFile(".hidden_file.txt");
            CreateFile("sub/.visible_file_in_sub.lua");
            CreateFile("sub/.hidden_dir/file.txt");
            CreateFile(".git/config");

            var files = FileCollect.CollectRelativeFilePaths(_tempTestDirectory, "*.*");

            Assert.Single(files);
            Assert.Contains(FilePath("visible.txt"), files);
            Assert.DoesNotContain(FilePath(".hidden_file.txt"), files);
            Assert.DoesNotContain(FilePath("sub/.visible_file_in_sub.lua"), files);
            Assert.DoesNotContain(FilePath("sub/.hidden_dir/file.txt"), files);
            Assert.DoesNotContain(FilePath(".git/config"), files);
        }

        [Fact]
        public void NormalizeFiles_EmptyArray_ReturnsEmptyDictionary()
        {
            var result = FileCollect.NormalizeFiles(_tempTestDirectory, []);
            Assert.Empty(result);
        }

        [Fact]
        public void NormalizeFiles_SingleFile_ReturnsCorrectMapping()
        {
            string fullPath = CreateFile("test.txt");
            var result = FileCollect.NormalizeFiles(_tempTestDirectory, [fullPath]);

            Assert.Single(result);
            Assert.Equal("test.txt", result[fullPath]);
        }

        [Fact]
        public void NormalizeFiles_MultipleFiles_ReturnsCorrectMappings()
        {
            string fullPath1 = CreateFile("folder/file1.lua");
            string fullPath2 = CreateFile("folder/sub/file2.json");

            var result = FileCollect.NormalizeFiles(_tempTestDirectory, [fullPath1, fullPath2]);

            Assert.Equal(2, result.Count);
            Assert.Equal("folder/file1.lua", result[fullPath1]);
            Assert.Equal("folder/sub/file2.json", result[fullPath2]);
        }

        [Fact]
        public void NormalizeFiles_BaseDirectoryWithTrailingSeparator_WorksCorrectly()
        {
            string fullPath = CreateFile("file.txt");
            string baseDirPathWithSeparator = _tempTestDirectory + Path.DirectorySeparatorChar;
            var result = FileCollect.NormalizeFiles(baseDirPathWithSeparator, [fullPath]);

            Assert.Single(result);
            Assert.Equal("file.txt", result[fullPath]);
        }

        [Fact]
        public void NormalizeFiles_BaseDirectoryWithoutTrailingSeparator_WorksCorrectly()
        {
            string fullPath = CreateFile("file.txt");
            string baseDirPathWithoutSeparator = _tempTestDirectory.TrimEnd(Path.DirectorySeparatorChar);
            var result = FileCollect.NormalizeFiles(baseDirPathWithoutSeparator, [fullPath]);

            Assert.Single(result);
            Assert.Equal("file.txt", result[fullPath]);
        }
        
        [Fact]
        public void CreateFileList_EmptyDictionary_ReturnsEmptyList()
        {
            var result = FileCollect.CreateFileList(new Dictionary<string, string>());
            Assert.Empty(result);
        }

        [Fact]
        public void CreateFileList_NoSuffixFilter_ReturnsAllValues()
        {
            var filesDict = new Dictionary<string, string>
            {
                {"full/path/file1.lua", "rel/path/file1.lua"},
                {"full/path/file2.json", "rel/path/file2.json"}
            };
            var result = FileCollect.CreateFileList(filesDict);

            Assert.Equal(2, result.Count);
            Assert.Contains("rel/path/file1.lua", result);
            Assert.Contains("rel/path/file2.json", result);
        }

        [Fact]
        public void CreateFileList_WithSuffixFilter_ReturnsMatchingValues()
        {
            var filesDict = new Dictionary<string, string>
            {
                {"full/path/file1.lua", "rel/path/file1.lua"},
                {"full/path/file2.json", "rel/path/file2.json"},
                {"full/path/test.lua", "rel/path/test.lua"}
            };
            var result = FileCollect.CreateFileList(filesDict, ".lua");

            Assert.Equal(2, result.Count);
            Assert.Contains("rel/path/file1.lua", result);
            Assert.Contains("rel/path/test.lua", result);
            Assert.DoesNotContain("rel/path/file2.json", result);
        }

        [Fact]
        public void CreateFileList_CaseInsensitiveSuffixFilter_ReturnsMatchingValues()
        {
            var filesDict = new Dictionary<string, string>
            {
                {"full/path/file1.LUA", "rel/path/file1.LUA"},
                {"full/path/file2.json", "rel/path/file2.json"}
            };
            var result = FileCollect.CreateFileList(filesDict, ".lua");

            Assert.Single(result);
            Assert.Contains("rel/path/file1.LUA", result);
        }

        [Fact]
        public void CreateFileCache_EmptyDictionary_ReturnsEmptyCache()
        {
            var cache = FileCollect.CreateFileCache(new Dictionary<string, string>(), ".json", content => content);
            Assert.Empty(cache);
        }

        [Fact]
        public void CreateFileCache_MatchingFiles_PopulatesCacheCorrectly()
        {
            string fullPath1 = CreateFile("data/config.json", "{ \"key\": \"value1\" }");
            string fullPath2 = CreateFile("data/settings.json", "{ \"key\": \"value2\" }");
            string baseDir = _tempTestDirectory;

            var filesDict = new Dictionary<string, string>
            {
                {fullPath1, "data/config.json"},
                {fullPath2, "data/settings.json"}
            };

            var cache = FileCollect.CreateFileCache(filesDict, ".json", content =>
            {
                dynamic? json = JsonConvert.DeserializeObject(content);
                return json?.key.ToString();
            });

            Assert.Equal(4, cache.Count);
            Assert.Contains("data/config.json", cache.Keys);
            Assert.Equal("value1", cache["data/config.json"]);
            Assert.Contains("data/config", cache.Keys);
            Assert.Equal("value1", cache["data/config"]);

            Assert.Contains("data/settings.json", cache.Keys);
            Assert.Equal("value2", cache["data/settings.json"]);
            Assert.Contains("data/settings", cache.Keys);
            Assert.Equal("value2", cache["data/settings"]);
        }

        [Fact]
        public void CreateFileCache_ProcessorReturnsNull_SkipsFile()
        {
            string fullPath = CreateFile("data/invalid.json", "invalid json");
            string baseDir = _tempTestDirectory;

            var filesDict = new Dictionary<string, string>
            {
                {fullPath, "data/invalid.json"}
            };

            var cache = FileCollect.CreateFileCache<object>(filesDict, ".json", content => null);
            Assert.Empty(cache);
        }

        [Fact]
        public void CreateFileCache_FileWithIndexName_AddsDirectoryKey()
        {
            string fullPath = CreateFile("menu/index.lua", "return { type = 'menu' }");
            string baseDir = _tempTestDirectory;

            var filesDict = new Dictionary<string, string>
            {
                {fullPath, "menu/index.lua"}
            };

            var cache = FileCollect.CreateFileCache(filesDict, ".lua", content => content);

            Assert.Equal(3, cache.Count);
            Assert.Contains("menu/index.lua", cache.Keys);
            Assert.Contains("menu/index", cache.Keys);
            Assert.Contains("menu", cache.Keys);
            Assert.Equal("return { type = 'menu' }", cache["menu"]);
        }

        [Fact]
        public void CreateFileCache_ErrorReadingFile_LogsErrorAndSkipsFile()
        {
            string fullPath = Path.Combine(_tempTestDirectory, "inaccessible.txt");
            Directory.CreateDirectory(fullPath);

            var filesDict = new Dictionary<string, string>
            {
                {fullPath, "inaccessible.txt"}
            };

            var cache = FileCollect.CreateFileCache<string>(filesDict, ".txt", content => content);
            Assert.Empty(cache);
            Assert.Contains($"[FileCache] Error processing file '{fullPath}': Access to the path '{fullPath}' is denied.", _consoleOutput.ToString());
        }
    }
}