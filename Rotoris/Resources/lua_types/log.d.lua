--- @class Rotoris.LuaLog
--- @field print fun(self: Rotoris.LuaLog, text: string)
--- @field println fun(self: Rotoris.LuaLog, text: string)

--- @class Rotoris.LuaLog
local log = {}
log:print("This is a log message.")
log:println("This is a log message with a newline.")
