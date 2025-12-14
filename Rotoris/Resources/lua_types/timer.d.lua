--- @class Rotoris.LuaModules.LuaTimer
--- @field delay fun(self: Rotoris.LuaModules.LuaTimer, milliseconds: number) Blocks the current execution thread for the specified number of milliseconds. **Use with caution as this is a blocking call and may freeze the Lua execution context.**
--- @field new_ticker fun(self: Rotoris.LuaModules.LuaTimer, func: fun(ctx: Rotoris.LuaModules.LuaTimer.TickerContext), milliseconds: number): Rotoris.LuaModules.LuaTimer.Ticker Creates and returns a new recurring timer (Ticker) instance.

--- @class Rotoris.LuaModules.LuaTimer.Ticker
--- @field start fun(self: Rotoris.LuaModules.LuaTimer.Ticker) Starts the Ticker, causing it to repeatedly call the associated Lua function at the set interval. Does nothing if the Ticker is already running.

--- @class Rotoris.LuaModules.LuaTimer.TickerContext
--- @field done fun(self: Rotoris.LuaModules.LuaTimer.TickerContext) Stops the current Ticker from making any further repeated calls. This is the recommended way to stop the timer from within the Lua function it calls.

--- @class Rotoris.LuaModules.LuaTimer
timer = {}

local counter = 0
local my_ticker = timer:new_ticker(function(ctx)
    counter = counter + 1
    print("Ticker fired: " .. counter)
    if counter >= 10 then
        ctx:done()
        print("Ticker stopped.")
    end
end, 1000)
my_ticker:start()