--- Provides system-level functionalities such as executing commands, managing external processes, opening files/URLs, and non-blocking task publishing.
--- @class Rotoris.LuaSystem
--- @field exec fun(self:Rotoris.LuaSystem, command: string, ...: string): Rotoris.LuaSystem.Process Executes a system command or program with optional arguments. Returns a Process object that can be started asynchronously.
--- @field get_executing fun(self:Rotoris.LuaSystem, id: string, callback: fun(ctx: Rotoris.LuaSystem.Process.ProcessContext)) Attempts to find an executing process by its ID and calls a Lua callback function with its context if found.
--- @field task fun(self:Rotoris.LuaSystem, actionId: string) Publishes an action ID for non-blocking execution in another VM/context. This is non-blocking.
--- @field delay fun(self:Rotoris.LuaSystem, milliseconds: number) @deprecated Use timer:delay() instead. Blocks the current execution thread for the specified number of milliseconds.
--- @field open fun(self:Rotoris.LuaSystem, targetPath: string) Opens a file or URL with the default associated application (using ShellExecute).

--- @class Rotoris.LuaSystem.Process
--- @field Id string A unique string identifier for the process instance.
--- @field start fun(self:Rotoris.LuaSystem.Process, table: Rotoris.LuaSystem.Process.Options?) Starts the external process execution. This call is blocking (synchronous) until the process exits or times out.
--- @field cancel fun(self:Rotoris.LuaSystem.Process) Attempts to cancel the blocking wait for the process to complete, causing the `start` function to return immediately.

--- @class Rotoris.LuaSystem.Process.Options
--- @field TimeoutMs? number The maximum time (in milliseconds) to wait for the process to exit. If omitted, it waits indefinitely.
--- @field OnStart? fun(ctx: Rotoris.LuaSystem.Process.ProcessContext, id: string) Called immediately after the process successfully starts.
--- @field OnOutput? fun(ctx: Rotoris.LuaSystem.Process.ProcessContext, data: string) Called asynchronously when new standard output data is received.
--- @field OnError? fun(ctx: Rotoris.LuaSystem.Process.ProcessContext, data: string) Called asynchronously when new standard error data is received.
--- @field OnTimeout? fun() Called if the process execution times out according to `TimeoutMs`.

--- @class Rotoris.LuaSystem.Process.ProcessContext
--- @field get_pid fun(self: Rotoris.LuaSystem.Process.ProcessContext): number Returns the operating system Process ID (PID). Throws an error if the process has already exited.
--- @field has_exited fun(self: Rotoris.LuaSystem.Process.ProcessContext): boolean Checks if the process has exited. Throws an error if the process has already exited.
--- @field get_exit_code fun(self: Rotoris.LuaSystem.Process.ProcessContext): number Returns the exit code of the process. Throws an error if the process has already exited.
--- @field is_running fun(self: Rotoris.LuaSystem.Process.ProcessContext): boolean Returns true if the process is currently running. Throws an error if the process has already exited.
--- @field write fun(self: Rotoris.LuaSystem.Process.ProcessContext, data: string): boolean Writes the given string data to the standard input (stdin) of the process. Returns true on success.
--- @field write_line fun(self: Rotoris.LuaSystem.Process.ProcessContext, data: string): boolean Writes the given string data followed by a newline to stdin. Returns true on success.
--- @field standard_input_close fun(self: Rotoris.LuaSystem.Process.ProcessContext): boolean Closes the standard input stream of the process. Returns true on success.
--- @field done fun(self: Rotoris.LuaSystem.Process.ProcessContext): boolean Signals the process execution to stop waiting (if currently blocked) and ensures cleanup. Returns true on success.
