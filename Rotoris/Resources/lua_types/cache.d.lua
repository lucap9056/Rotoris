--- A Lua module for caching arbitrary key-value pairs in a thread-safe manner.
--- @class Rotoris.LuaCache
--- @field set fun(self:Rotoris.LuaCache, key: string, value: any) Sets a value in the cache for the specified key.
--- @field get fun(self:Rotoris.LuaCache, key: string): any | nil Retrieves a value from the cache for the specified key. Returns nil if the key does not exist.
--- @field remove fun(self:Rotoris.LuaCache, key: string): any | nil Removes a value from the cache for the specified key and returns it. Returns nil if the key does not exist.
--- @field exists fun(self:Rotoris.LuaCache, key: string): boolean Checks if a key exists in the cache.
--- @field clear fun(self:Rotoris.LuaCache, ) Clears all entries in the cache.
--- @field exclusive fun(self:Rotoris.LuaCache, callback: fun()) Executes a callback function exclusively, ensuring no other operations can occur on the cache during its execution.

--- @class Rotoris.LuaCache
local cache = {}

if cache:exists("exampleKey") then
    local value = cache:get("exampleKey")
    print("Value: " .. tostring(value))
    cache:remove("exampleKey")
else
    cache:set("exampleKey", "exampleValue")
end

cache:exclusive(function()
    cache:set("exclusiveKey", "exclusiveValue")
    local exclusiveValue = cache:get("exclusiveKey")
    print("Exclusive Value: " .. tostring(exclusiveValue))
end)
