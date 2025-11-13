using NLua;

namespace Rotoris.LuaModules
{
    /*
--- A Lua module for caching arbitrary key-value pairs in a thread-safe manner.
--- @class Rotoris.LuaCache
--- @field set fun(self:Rotoris.LuaCache, key: string, value: any): void Sets a value in the cache for the specified key.
--- @field get fun(self:Rotoris.LuaCache, key: string): any | nil Retrieves a value from the cache for the specified key. Returns nil if the key does not exist.
--- @field remove fun(self:Rotoris.LuaCache, key: string): any | nil Removes a value from the cache for the specified key and returns it. Returns nil if the key does not exist.
--- @field exists fun(self:Rotoris.LuaCache, key: string): boolean Checks if a key exists in the cache.
--- @field clear fun(self:Rotoris.LuaCache, ): void Clears all entries in the cache.
--- @field exclusive fun(self:Rotoris.LuaCache, callback: fun()): void Executes a callback function exclusively, ensuring no other operations can occur on the cache during its execution.
     */
    public class LuaCache()
    {
        private readonly Dictionary<string, object> cache = [];
        private readonly Lock lockObject = new();
        public void set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");
            }
            lock (lockObject)
            {
                cache[key] = value;
            }
        }
        public object? get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            lock (lockObject)
            {
                if (cache.TryGetValue(key, out object? value))
                {
                    return value;
                }
                return null;
            }
        }
        public object? remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            lock (lockObject)
            {
                if (cache.Remove(key, out var value))
                {
                    return value;
                }
                return null;
            }
        }
        public bool exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            lock (lockObject)
            {
                return cache.ContainsKey(key);
            }
        }
        public void clear()
        {
            lock (lockObject)
            {
                cache.Clear();
            }
        }

        public void exclusive(LuaFunction callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Exclusive callback function cannot be null.");
            }
            lock (lockObject)
            {
                callback.Call();
            }
        }
    }
}
