using NLua;

namespace Rotoris.LuaModules.LuaUtils
{
    /*
--- A wrapper for asynchronous operations that return a Result.
--- @class Rotoris.Async
--- @field wait fun(self: Rotoris.Async): Rotoris.Result The method to wait for the asynchronous operation to complete and get the Result.
--- @field match fun(self: Rotoris.Async, luaResultHandlers: Rotoris.Result.MatchHandlers, timeoutMilliseconds?: number): any|nil The method to match the Result with Lua handlers, with an optional timeout.
--- @see Rotoris.Result
--- @see Rotoris.Result.MatchHandlers
     */
    public class Async<T>(Task<T> task)
    {

        public Result<T> wait()
        {
            return Result<T>.Run(task.GetAwaiter().GetResult);
        }
        public object? match(LuaTable luaResultHandlers, int timeoutMilliseconds = 1000)
        {
            if (luaResultHandlers == null)
            {
                return null;
            }

            if (!task.Wait(timeoutMilliseconds))
            {
                if (luaResultHandlers["Err"] is LuaFunction Err)
                {
                    string timeoutMessage = $"Operation timed out after {timeoutMilliseconds} milliseconds.";
                    Err.Call(timeoutMessage);
                }
            }
            else
            {
                return Result<T>.Run(task.GetAwaiter().GetResult).match(luaResultHandlers);
            }

            return null;
        }
    }
}
