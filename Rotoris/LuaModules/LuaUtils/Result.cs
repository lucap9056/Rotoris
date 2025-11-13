using NLua;

namespace Rotoris.LuaModules.LuaUtils
{
    /*
--- A Result type that can be either Ok or Err.
--- @class Rotoris.Result
--- @field is_ok boolean True if the result is Ok, false if it is Err.
--- @field is_err boolean True if the result is Err, false if it is Ok.
--- @field ok fun(self: Rotoris.Result): any|nil Returns the value if the result is Ok, or nil if it is Err.
--- @field err fun(self: Rotoris.Result): string|nil Returns the error message if the result is Err, or nil if it is Ok.
--- @field unwrap fun(self: Rotoris.Result): any Unwraps the value if the result is Ok, or throws an error if it is Err.
--- @field match fun(self: Rotoris.Result, handlers: Rotoris.Result.MatchHandlers): any Matches the result and calls the appropriate handler.
--- @see Rotoris.Result.MatchHandlers
     */
    public class Result<T>(bool isOk, T? Value, string? Error = null)
    {
        public static Result<T> Ok(T value)
        {
            return new Result<T>(true, value);
        }

        public static Result<T> Err(string err)
        {
            return new Result<T>(false, default, err);
        }
        public static Result<T> Run(Func<T> action)
        {
            try
            {
                return Ok(action());
            }
            catch (Exception e)
            {
                return Err(e.Message);
            }
        }

        public bool is_ok { get => isOk; }

        public bool is_err { get => !isOk; }

        public T? ok() => isOk ? Value : default;

        public string? err() => isOk ? null : Error;

        public T unwrap()
        {
            if (isOk)
            {
                return Value!;
            }

            throw new InvalidOperationException($"Cannot unwrap value. The result is an error: {Error ?? "Unknown error"}");
        }

        /*
--- @class Rotoris.Result.MatchHandlers
--- @field Ok fun(value: any): any Function to handle the Ok case.
--- @field Err fun(error: string): any Function to handle the Err case.
--- @return any The result of calling the appropriate handler function.
         */
        public object? match(LuaTable luaResultHandlers)
        {
            LuaFunction? Ok = luaResultHandlers["Ok"] as LuaFunction;
            LuaFunction? Err = luaResultHandlers["Err"] as LuaFunction;

            if (isOk)
            {
                return (Ok?.Call(Value) is object[] values && values.Length > 0) ? values[0] : null;
            }
            else
            {
                return (Err?.Call(Error) is object[] values && values.Length > 0) ? values[0] : null; ;
            }
        }
    }
}
