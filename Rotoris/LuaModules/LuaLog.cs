using Rotoris.Logger;

namespace Rotoris.LuaModules
{
    /*
--- @class Rotoris.LuaLog
--- @field print fun(self: Rotoris.LuaLog, text: string)
--- @field println fun(self: Rotoris.LuaLog, text: string)
     */
    public class LuaLog
    {
        public static readonly string GlobalMame = "log";
        public void print(string text)
        {
            Log.Write(text);
        }
        public void println(string text)
        {
            Log.WriteLine(text);
        }
    }
}
