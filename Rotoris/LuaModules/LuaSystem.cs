using System.Collections.Concurrent;
using Rotoris.Logger;
using NLua;
using System.IO;

namespace Rotoris.LuaModules
{
    /*
--- Provides system-level functionalities such as executing commands and opening files or URLs.
--- @class Rotoris.LuaSystem
--- @field exec fun(self:Rotoris.LuaSystem, command: string, ...: string): Process? Executes a system command or opens a file with optional arguments.
--- @field open fun(self:Rotoris.LuaSystem, targetPath: string) Opens a file or URL with the default associated application.

---@class Process
     */
    public class LuaSystem
    {
        public static readonly string GlobalName = "sys";
        private readonly List<Process> processes = [];
        public Process exec(string command, params string[] arguments)
        {

            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException("Command or file path cannot be empty.", nameof(command));
            }

            Process process = new(command, arguments);
            processes.Add(process);
            process.ProcessDone += (proc) =>
            {
                processes.Remove(proc);
            };
            return process;
        }

        public void get_executing(string id, LuaFunction callback)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Process id cannot be empty.", nameof(id));
            }
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            Process? process = processes.Find(p => p.Id == id);

            if (process == null)
            {
                Log.Error($"Process with id '{id}' not found or already completed.");
                return;
            }

            Process.ProcessContext? context = process.Context;

            if (context == null)
            {
                Log.Error($"Process with id '{id}' has no active context (might not have started yet).");
                return;
            }

            try
            {
                callback.Call(context);

                Log.Info($"Successfully executed callback for process '{id}'.");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred during Lua callback execution for id '{id}'. Details: {ex.Message}");
            }
        }

        public void task(string actionId)
        {
            Log.Info($"Publishing task action '{actionId}' to EventAggregator.");
            Task.Run(() =>
            {
                EventAggregator.PublishExecuteAction(actionId);
            });
        }

        public void delay(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }

        public void open(string targetPath)
        {

            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Log.Error("The path or URL to open cannot be empty.");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(targetPath)
                {
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                });

                Log.Info($"Successfully requested to open '{targetPath}'.");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Log.Error($"Failed to open '{targetPath}'. Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An unexpected error occurred while trying to open '{targetPath}'. Details: {ex.Message}");
            }
        }

        public void Clear()
        {
            foreach (Process process in processes)
            {
                process.Dispose();
            }
            processes.Clear();
        }

        public class Process(string command, params string[] arguments) : IDisposable
        {
            public delegate void ProcessDoneEventHandler(Process proc);
            public event ProcessDoneEventHandler? ProcessDone;
            public readonly string Id = Guid.NewGuid().ToString("N");
            private ProcessContext? context;
            private ManualResetEventSlim? manualEvent;

            public struct Options
            {
                public double TimeoutMs { get; set; }
                public LuaFunction? OnStart { get; set; }
                public LuaFunction? OnOutput { get; set; }
                public LuaFunction? OnError { get; set; }
                public LuaFunction? OnTimeout { get; set; }
            }

            public void start(LuaTable? table)
            {
                if (manualEvent != null)
                {
                    throw new InvalidOperationException("Process has already been started.");
                }

                try
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new()
                    {
                        FileName = command,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = false
                    };

                    if (arguments != null && arguments.Length > 0)
                    {
                        startInfo.Arguments = string.Join(" ", arguments);
                    }

                    Log.Info($"Executing: '{startInfo.FileName}' with arguments: '{startInfo.Arguments}'");

                    if (System.Diagnostics.Process.Start(startInfo) is System.Diagnostics.Process proc)
                    {

                        manualEvent = new();
                        ProcessContext ctx = new(proc, manualEvent);
                        proc.EnableRaisingEvents = true;
                        proc.Exited += (s, e) => manualEvent.Set();

                        if (table?["OnStart"] is LuaFunction onStart)
                        {
                            onStart.Call(ctx, Id);
                        }

                        LuaFunction? onError = table?["OnError"] as LuaFunction;
                        LuaFunction? onOutput = table?["OnOutput"] as LuaFunction;

                        if (onError == null && onOutput == null)
                        {
                            return;
                        }

                        MessageQueue messages = new(ctx, onError, onOutput);


                        System.Diagnostics.DataReceivedEventHandler output = (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                        {
                            messages.Append(e.Data ?? "");
                        };

                        System.Diagnostics.DataReceivedEventHandler err = (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                        {
                            messages.Append(e.Data ?? "", true);
                        };

                        if (onOutput != null)
                        {
                            proc.OutputDataReceived += output;
                            proc.BeginOutputReadLine();
                        }

                        if (onError != null)
                        {
                            proc.ErrorDataReceived += err;
                            proc.BeginErrorReadLine();
                        }

                        context = ctx;

                        if (table?["TimeoutMs"] != null)
                        {
                            int timeoutMs = Convert.ToInt32(table["TimeoutMs"]);

                            bool completed = manualEvent.Wait(timeoutMs);

                            if (!completed)
                            {
                                Log.Error($"Execution of '{command}' timed out after {timeoutMs}ms.");
                                if (table?["OnTimeout"] is LuaFunction onTimeout)
                                {
                                    onTimeout.Call();
                                }
                            }
                        }
                        else
                        {
                            manualEvent.Wait();
                        }

                        proc.OutputDataReceived -= output;
                        proc.ErrorDataReceived -= err;

                        if (!proc.HasExited)
                        {
                            proc.Kill();
                        }

                        ProcessDone?.Invoke(this);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The process for '{command}' failed to start, returned null.");
                    }

                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    throw new InvalidOperationException($"Failed to execute '{command}'. Reason: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An unexpected error occurred while executing '{command}'. Reason: {ex.Message}", ex);
                }
            }

            public ProcessContext? Context
            {
                get => context;
            }

            public void cancel()
            {
                if (manualEvent == null)
                {
                    ProcessDone?.Invoke(this);
                }
                else
                {
                    manualEvent.Set();
                }
            }
            public void Dispose()
            {
                manualEvent?.Set();
            }

            class MessageQueue(ProcessContext ctx, LuaFunction? onError, LuaFunction? onOutput)
            {
                private readonly ConcurrentQueue<Message> queue = [];
                private int isProcessing = 0;
                private void ProcessQueue()
                {
                    try
                    {
                        while (queue.TryDequeue(out Message? message))
                        {
                            if (message.IsError)
                            {
                                onError?.Call(ctx, message.Content);
                            }
                            else
                            {
                                onOutput?.Call(ctx, message.Content);
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Exchange(ref isProcessing, 0);
                    }
                }

                public void Append(string content, bool isError = false)
                {
                    Message message = new(content, isError);
                    queue.Enqueue(message);

                    if (Interlocked.CompareExchange(ref isProcessing, 1, 0) == 0)
                    {
                        Task.Run(ProcessQueue);
                    }
                }
                public class Message(string content, bool isError = false)
                {
                    public bool IsError { get; init; } = isError;
                    public string Content { get; init; } = content;
                }
            }
            public class ProcessContext(System.Diagnostics.Process process, ManualResetEventSlim manualEvent)
            {
                public int get_pid()
                {
                    if (manualEvent.IsSet)
                    {
                        throw new InvalidOperationException("Cannot get PID of a process that has already exited.");
                    }
                    return process.Id;
                }
                public bool has_exited()
                {
                    if (manualEvent.IsSet)
                    {
                        throw new InvalidOperationException("Cannot check exit status of a process that has already exited.");
                    }
                    return process.HasExited;
                }
                public int get_exit_code()
                {
                    if (manualEvent.IsSet)
                    {
                        throw new InvalidOperationException("Cannot get exit code of a process that has already exited.");
                    }
                    return process.ExitCode;
                }
                public bool is_running()
                {
                    if (manualEvent.IsSet)
                    {
                        throw new InvalidOperationException("Cannot check running status of a process that has already exited.");
                    }
                    return !process.HasExited;
                }
                public bool write(string data)
                {
                    if (manualEvent.IsSet)
                    {
                        Log.Warning("Cannot write to standard input of a process that has already exited.");
                        return false;
                    }
                    using StreamWriter sw = process.StandardInput;
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.Write(data);
                        return true;
                    }
                    return false;
                }
                public bool write_line(string data)
                {
                    if (manualEvent.IsSet)
                    {
                        Log.Warning("Cannot write to standard input of a process that has already exited.");
                        return false;
                    }
                    using StreamWriter sw = process.StandardInput;
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(data);
                        return true;
                    }
                    return false;
                }
                public bool standard_input_close()
                {
                    if (manualEvent.IsSet)
                    {
                        Log.Warning("Cannot close standard input of a process that has already exited.");
                        return false;
                    }
                    process.StandardInput.Close();
                    return true;
                }
                public bool done()
                {
                    if (manualEvent.IsSet)
                    {
                        Log.Warning("Process has already exited.");
                        return false;
                    }
                    manualEvent.Set();
                    return true;
                }

            }

        }
    }
}
