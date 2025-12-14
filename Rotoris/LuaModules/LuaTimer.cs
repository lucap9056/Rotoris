using NLua;
using Rotoris.Logger;
using System.Diagnostics;

namespace Rotoris.LuaModules
{
    /*
--- @class Rotoris.LuaTimer
--- @field delay fun(self: Rotoris.LuaTimer, milliseconds: number) Blocks the current execution thread for the specified number of milliseconds. **Use with caution as this is a blocking call and may freeze the Lua execution context.**
--- @field new_ticker fun(self: Rotoris.LuaTimer, func: fun(ctx: Rotoris.LuaTimer.TickerContext), milliseconds: number): Rotoris.LuaTimer.Ticker Creates and returns a new recurring timer (Ticker) instance.
     */
    public class LuaTimer
    {
        public readonly static string GlobalName = "timer";

        private readonly HashSet<Ticker> tickers = [];
        public void delay(int milliseconds)
        {
            Task.Delay(milliseconds).Wait();
        }

        /*   
--- @class Rotoris.LuaTimer.Ticker.Options
--- @field Func fun(ctx: Rotoris.LuaTimer.TickerContext, state:any):any The Lua function to be executed every time the timer interval elapses.
--- @field Delay number The time interval, in milliseconds (ms), between consecutive calls to the `Func`.
         */
        public Ticker new_ticker(LuaTable options)
        {
            LuaFunction? callback = options["Func"] as LuaFunction;
            int delay = Convert.ToInt32(options["Delay"]);

            ArgumentNullException.ThrowIfNull(callback);
            ArgumentOutOfRangeException.ThrowIfNegative(delay);

            Ticker ticker = new(this, callback, delay);
            lock (tickers)
            {
                tickers.Add(ticker);
            }
            return ticker;
        }
        private void RemoveTicker(Ticker ticker)
        {
            lock (tickers)
            {
                tickers.Remove(ticker);
            }
        }
        public void Clear()
        {
            List<Ticker> tickersToDispose;

            lock (tickers)
            {
                tickersToDispose = [.. tickers];
                tickers.Clear();
            }

            foreach (var ticker in tickersToDispose)
            {
                ticker.Dispose();
            }
        }

        /*
--- @class Rotoris.LuaTimer.Ticker
--- @field start fun(self: Rotoris.LuaTimer.Ticker) Starts the Ticker, causing it to repeatedly call the associated Lua function at the set interval. Does nothing if the Ticker is already running.
         */
        public class Ticker(LuaTimer parent, LuaFunction func, int milliseconds) : IDisposable
        {
            private readonly TickerContext ctx = new();
            private bool started = false;
            public void start()
            {
                if (started) return;
                started = true;

                Stopwatch stopwatch = new();
                stopwatch.Start();

                long intervalTicks = milliseconds * TimeSpan.TicksPerMillisecond;
                long nextExecutionTimeTicks = stopwatch.ElapsedTicks + intervalTicks;

                bool shouldStop = false;
                ctx.StopTicker += (sender, e) =>
                {
                    shouldStop = true;
                };

                object? state = null;

                while (!shouldStop)
                {
                    long currentTicks = stopwatch.ElapsedTicks;
                    if (currentTicks >= nextExecutionTimeTicks)
                    {
                        try
                        {
                            object[] results = func.Call(ctx, state);
                            if (results.Length > 0 && results[0] != null)
                            {
                                state = results[0];
                            }
                            while (nextExecutionTimeTicks <= currentTicks)
                            {
                                nextExecutionTimeTicks += intervalTicks;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"Ticker callback error: {ex.Message}");
                            ctx.done();
                            break;
                        }
                    }
                    else
                    {
                        long ticksToWait = nextExecutionTimeTicks - currentTicks;
                        int millisecondsToWait = (int)(ticksToWait / TimeSpan.TicksPerMillisecond);

                        if (millisecondsToWait > 0)
                        {
                            Thread.Sleep(millisecondsToWait);
                        }
                    }
                }

                parent.RemoveTicker(this);
            }

            public void Dispose()
            {
                ctx.done();
            }
        }
        /*
--- @class Rotoris.LuaTimer.TickerContext
--- @field done fun(self: Rotoris.LuaTimer.TickerContext) Stops the current Ticker from making any further repeated calls. This is the recommended way to stop the timer from within the Lua function it calls.
         */
        public class TickerContext
        {
            public event EventHandler? StopTicker;
            public void done()
            {
                StopTicker?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}
