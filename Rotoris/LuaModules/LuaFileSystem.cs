using Rotoris.LuaModules.LuaUtils;
using System.IO;

namespace Rotoris.LuaModules
{
    /*
--- A module for file system operations.
--- @class Rotoris.LuaFileSystem
--- @field read_file fun(self:Rotoris.LuaFileSystem, path: string): string Reads the content of a file at the specified path.
--- @field read_file_bytes fun(self:Rotoris.LuaFileSystem, path: string): Bytes Reads the content of a file at the specified path as bytes.
--- @field write_file fun(self:Rotoris.LuaFileSystem, path: string, content: string) Writes the specified content to a file at the specified path.
--- @field write_file_bytes fun(self:Rotoris.LuaFileSystem, path: string, content: Bytes) Writes the specified byte content to a file at the specified path.
--- @field file_exists fun(self:Rotoris.LuaFileSystem, path: string): boolean Checks if a file exists at the specified path.
--- @field file_delete fun(self:Rotoris.LuaFileSystem, path: string) Deletes the file at the specified path.
     */
    public class LuaFileSystem
    {
        public string read_file(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"[ERROR] File not found at path: {fullPath}");
            }
            return File.ReadAllText(fullPath);
        }

        public Bytes read_file_bytes(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"[ERROR] File not found at path: {fullPath}");
            }
            byte[] bytes = File.ReadAllBytes(fullPath);
            return new Bytes(bytes);
        }

        public void write_file(string path, string content)
        {
            string fullPath = Path.GetFullPath(path);
            File.WriteAllText(fullPath, content);
        }

        public void write_file_bytes(string path, Bytes content)
        {
            string fullPath = Path.GetFullPath(path);
            File.WriteAllBytes(fullPath, content.Data);
        }
        public bool file_exists(string path)
        {
            string fullPath = Path.GetFullPath(path);
            return File.Exists(fullPath);
        }

        public void file_delete(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                throw new FileNotFoundException($"[ERROR] File not found at path: {fullPath}");
            }
        }
    }
}
