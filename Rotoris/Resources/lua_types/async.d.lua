--- A wrapper for asynchronous operations that return a Result.
--- @class Rotoris.Async
--- @field wait fun(self: Rotoris.Async): Rotoris.Result The method to wait for the asynchronous operation to complete and get the Result.
--- @field match fun(self: Rotoris.Async, luaResultHandlers: Rotoris.Result.MatchHandlers, timeoutMilliseconds?: number): any|nil The method to match the Result with Lua handlers, with an optional timeout.
--- @see Rotoris.Result
--- @see Rotoris.Result.MatchHandlers

--- @class Rotoris.Async
local task = {}

local result = task:wait()

result:match({
    Ok = function(value)
        print("Operation succeeded with value: " .. tostring(value))
    end,
    Err = function(errorMessage)
        print("Operation failed with error: " .. errorMessage)
    end
})

task:match({
    Ok = function(value)
        print("Operation succeeded with value: " .. tostring(value))
    end,
    Err = function(errorMessage)
        print("Operation failed with error: " .. errorMessage)
    end
}, 5000)
