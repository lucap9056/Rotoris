using NLua;
using System.Collections.Concurrent;
using Rotoris.Logger;
using System.Text;

namespace Rotoris.LuaModules
{
    public class LuaVmPool : IDisposable
    {
        public delegate void LuaVmUpdateEventHandler(Lua vm);

        public event LuaVmUpdateEventHandler? LuaVmCreated;
        public event LuaVmUpdateEventHandler? LuaVmRemoving;

        private readonly SemaphoreSlim semaphore;
        private readonly ConcurrentDictionary<Lua, bool> busyVms = [];
        private readonly ConcurrentQueue<Lua> idleVms = [];
        private readonly Lock vmCountLock = new();
        private readonly int minimum;
        private readonly ConcurrentDictionary<Lua, DateTime> dynamicVmIdleTimers = new();
        private readonly TimeSpan dynamicVmTimeout;
        private readonly Timer? cleanupTimer;

        private int currentVmCount = 0;
        public LuaVmPool(int min, int max) : this(min, max, TimeSpan.FromSeconds(15)) { }
        public LuaVmPool(int min, int max, TimeSpan timeout)
        {
            if (min >= 0 && max >= min)
            {
                semaphore = new SemaphoreSlim(max, max);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(min), "min must be >= 0 and max must be >= min.");
            }

            minimum = min;
            dynamicVmTimeout = timeout;
            if (timeout > TimeSpan.Zero)
            {
                cleanupTimer = new Timer(CleanupDynamicVms, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            }
        }
        public void UseLua(Action<Lua> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            semaphore.Wait();

            Lua? vm = null;
            bool isDynamic = false;

            try
            {

                if (!dynamicVmIdleTimers.IsEmpty)
                {
                    foreach (var pair in dynamicVmIdleTimers)
                    {
                        if (dynamicVmIdleTimers.TryRemove(pair.Key, out _))
                        {
                            vm = pair.Key;
                            isDynamic = true;
                            break;
                        }
                    }
                }

                if (vm == null && idleVms.TryDequeue(out Lua? idleVm))
                {
                    vm = idleVm;
                }
                else if (vm == null)
                {
                    lock (vmCountLock)
                    {
                        vm = CreateNewVm();
                        isDynamic = currentVmCount > minimum;
                        Log.Info($"[LuaVmPool] Created new Lua VM. Current VM count: {currentVmCount} (Dynamic: {isDynamic})");
                    }
                }

                if (vm != null)
                {
                    busyVms.TryAdd(vm, isDynamic);
                    action(vm);
                }
            }
            finally
            {
                if (vm != null)
                {
                    if (isDynamic)
                    {
                        dynamicVmIdleTimers.TryAdd(vm, DateTime.UtcNow);
                    }
                    else
                    {
                        idleVms.Enqueue(vm);
                    }
                }

                semaphore.Release();
            }
        }

        public void UpdateVms(Action<Lua> action)
        {
            if (action == null)
            {
                return;
            }

            foreach (var vm in idleVms)
            {
                action(vm);
            }
        }
        public void ForEach(Action<Lua, bool> action)
        {
            if (action == null)
            {
                return;
            }

            foreach (var item in busyVms)
            {
                action(item.Key, item.Value);
            }
        }
        private Lua CreateNewVm()
        {
            var newVm = new Lua();
            LuaVmCreated?.Invoke(newVm);
            newVm.State.Encoding = Encoding.UTF8;
            currentVmCount++;
            return newVm;
        }
        private void CleanupDynamicVms(object? _)
        {
            foreach (var pair in dynamicVmIdleTimers)
            {
                if (DateTime.UtcNow - pair.Value > dynamicVmTimeout)
                {
                    Lua vm = pair.Key;
                    if (dynamicVmIdleTimers.TryRemove(vm, out var _))
                    {
                        lock (vmCountLock)
                        {
                            busyVms.TryRemove(vm, out var _);
                            LuaVmRemoving?.Invoke(vm);
                            vm.Dispose();
                            currentVmCount--;
                            Log.Info($"[LuaVmPool] Disposed a timed-out dynamic Lua VM. Current VM count: {currentVmCount}");
                        }
                    }
                }
            }
        }
        public void Dispose()
        {
            semaphore.Dispose();
            cleanupTimer?.Dispose();
            while (idleVms.TryDequeue(out Lua? vm))
            {
                LuaVmRemoving?.Invoke(vm);
                vm.Dispose();
            }
            foreach (var vm in dynamicVmIdleTimers.Keys)
            {
                LuaVmRemoving?.Invoke(vm);
                vm.Dispose();
            }
            dynamicVmIdleTimers.Clear();
            lock (vmCountLock)
            {
                currentVmCount = 0;
            }
        }
    }
}
