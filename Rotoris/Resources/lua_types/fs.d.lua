--- A module for file system operations.
--- @class Rotoris.LuaFileSystem
--- @field read_file fun(self:Rotoris.LuaFileSystem, path: string): string Reads the content of a file at the specified path.
--- @field read_file_bytes fun(self:Rotoris.LuaFileSystem, path: string): string Reads the content of a file at the specified path as bytes.
--- @field write_file fun(self:Rotoris.LuaFileSystem, path: string, content: string) Writes the specified content to a file at the specified path.
--- @field write_file_bytes fun(self:Rotoris.LuaFileSystem, path: string, content: string) Writes the specified byte content to a file at the specified path.
--- @field file_exists fun(self:Rotoris.LuaFileSystem, path: string): boolean Checks if a file exists at the specified path.
--- @field file_delete fun(self:Rotoris.LuaFileSystem, path: string) Deletes the file at the specified path.

--- @class Rotoris.LuaFileSystem
local fs = {}

if fs:file_exists("path/to/file.txt") then
    local content = fs:read_file("path/to/file.txt")
    print(content)
    fs:write_file("path/to/another_file.txt", content)
    fs:file_delete("path/to/file.txt")
end
