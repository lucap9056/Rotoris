namespace RotorisLib
{
    /// <summary>
    /// Implements a global low-level keyboard and mouse hook using a dedicated thread
    /// to ensure safe dispatcher access for event handling.
    /// </summary>
    public class GlobalInputHook : IDisposable
    {
        // -------------------------------------------------------------------
        // --- Windows API Constants (Hook Types and Messages) ---
        // -------------------------------------------------------------------

        // Hook Types
        private const int WH_KEYBOARD_LL = 13; // Low-Level Keyboard Hook
        private const int WH_MOUSE_LL = 14;    // Low-Level Mouse Hook

        // Keyboard Messages (wParam)
        private const IntPtr WM_KEYDOWN = 0x0100;
        private const IntPtr WM_KEYUP = 0x0101;

        // Mouse Messages (wParam)
        private const IntPtr WM_LBUTTONDOWN = 0x0201;
        private const IntPtr WM_LBUTTONUP = 0x0202;
        private const IntPtr WM_RBUTTONDOWN = 0x0204;
        private const IntPtr WM_RBUTTONUP = 0x0205;
        private const IntPtr WM_MBUTTONDOWN = 0x0207;
        private const IntPtr WM_MBUTTONUP = 0x0208;
        private const IntPtr WM_XBUTTONDOWN = 0x020B;
        private const IntPtr WM_XBUTTONUP = 0x020C;
        private const IntPtr WM_MOUSEWHEEL = 0x020A;
        private const IntPtr WM_MOUSEHWHEEL = 0x020E;

        // Virtual Key Codes (vkCode) for Scroll Wheels
        public const int VK_WHEEL_UP = 0xF0;
        public const int VK_WHEEL_DOWN = 0xF1;
        public const int VK_WHEEL_LEFT = 0xF2;
        public const int VK_WHEEL_RIGHT = 0xF3;

        // Virtual Key Codes (vkCode) for Mouse Buttons
        public const int VK_LBUTTON = 0x01;
        public const int VK_RBUTTON = 0x02;
        public const int VK_MBUTTON = 0x04;
        public const int VK_XBUTTON1 = 0x05;
        public const int VK_XBUTTON2 = 0x06;

        /// <summary>
        /// Delegate for the Low-Level Hook procedure.
        /// </summary>
        private delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        // -------------------------------------------------------------------
        // --- P/Invoke Structures ---
        // -------------------------------------------------------------------

        /// <summary>
        /// Contains information about a low-level keyboard input event.
        /// Renamed from KBDLLHOOKSTRUCT for clarity and C# convention.
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct KeyboardLowLevelHookStruct
        {
            public int VirtualKeyCode; // vkCode
            public int ScanCode;       // scanCode
            public int Flags;          // flags
            public int Time;           // time
            public IntPtr ExtraInfo;   // dwExtraInfo
        }

        /// <summary>
        /// Contains information about a low-level mouse input event.
        /// Renamed from MSLLHOOKSTRUCT for clarity and C# convention.
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct MouseLowLevelHookStruct
        {
            public System.Drawing.Point Point; // pt (System.Drawing.Point is simpler than creating a separate Point struct)
            public int MouseData;              // mouseData (contains wheel delta or X-button info)
            public int Flags;                  // flags
            public int Time;                   // time
            public IntPtr ExtraInfo;           // dwExtraInfo
        }

        // -------------------------------------------------------------------
        // --- P/Invoke Methods ---
        // -------------------------------------------------------------------

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // -------------------------------------------------------------------
        // --- Event Arguments & Events ---
        // -------------------------------------------------------------------

        /// <summary>
        /// Provides data for input events, including the key code and an action to block the input.
        /// </summary>
        /// <param name="KeyCode">The virtual key code of the event (can be a keyboard key or mouse button).</param>
        /// <param name="BlockAction">An action to call if the event subscriber wants to block (suppress) the input stroke.</param>
        public sealed class InputHookEventArgs(int VirtualKeyCode, Action BlockAction, Hotkey Key) : EventArgs
        {
            public int VirtualKeyCode { get; init; } = VirtualKeyCode;

            /// <summary>Gets an action to call if the event subscriber wants to block (suppress) the stroke.</summary>
            public Action ShouldBlock { get; init; } = BlockAction;
            public Hotkey Key { get; init; } = Key;
        }

        /// <summary>Occurs when an input (key or button) is pressed down system-wide.</summary>
        public event EventHandler<InputHookEventArgs>? KeyDown;

        /// <summary>Occurs when an input (key or button) is released system-wide.</summary>
        public event EventHandler<InputHookEventArgs>? KeyUp;

        // -------------------------------------------------------------------
        // --- Internal Members ---
        // -------------------------------------------------------------------

        private readonly Thread hookThread;
        private LowLevelHookProc? hookCallbackDelegate;
        private IntPtr keyboardHookHandle = IntPtr.Zero;
        private IntPtr mouseHookHandle = IntPtr.Zero;
        private System.Windows.Threading.Dispatcher? hookDispatcher;
        private volatile bool isHookRunning = false;
        private readonly Hotkey modifiers = new();
        // -------------------------------------------------------------------
        // --- Constructor & Disposable ---
        // -------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalInputHook"/> class and starts a dedicated thread for hook monitoring.
        /// </summary>
        public GlobalInputHook()
        {
            hookThread = new Thread(HookThreadStart)
            {
                IsBackground = true,
                Name = "GlobalInputHookThread"
            };
            hookThread.Start();

            while (!isHookRunning)
            {
                Thread.Sleep(1);
            }
        }
        private static IntPtr GetModuleHandleForHook()
        {
            System.Diagnostics.ProcessModule? currentModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            if (currentModule == null || currentModule.ModuleName == null)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] Failed to get current process module name.");
                return IntPtr.Zero;
            }
            return GetModuleHandle(currentModule.ModuleName);
        }

        /// <summary>
        /// The main method for the dedicated hook thread.
        /// This thread receives the low-level hook calls and runs the Dispatcher loop.
        /// </summary>
        private void HookThreadStart()
        {
            try
            {
                hookDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                hookCallbackDelegate = HookCallback;

                IntPtr moduleHandle = GetModuleHandleForHook();
                if (moduleHandle == IntPtr.Zero)
                {
                    isHookRunning = false;
                    return;
                }

                // 1. Keyboard Hook
                keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hookCallbackDelegate, moduleHandle, 0);
                // 2. Mouse Hook
                mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, hookCallbackDelegate, moduleHandle, 0);

                if (keyboardHookHandle == IntPtr.Zero || mouseHookHandle == IntPtr.Zero)
                {
                    int errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    System.Diagnostics.Debug.WriteLine($"[ERROR] HookThreadStart: Failed to install one or both hooks. Win32 Error Code: {errorCode}.");

                    if (keyboardHookHandle != IntPtr.Zero) UnhookWindowsHookEx(keyboardHookHandle);
                    if (mouseHookHandle != IntPtr.Zero) UnhookWindowsHookEx(mouseHookHandle);
                    isHookRunning = false;
                    return;
                }

                isHookRunning = true;
                System.Diagnostics.Debug.WriteLine("[LOG] Global keyboard and mouse hooks installed successfully.");

                // Start the message loop
                System.Windows.Threading.Dispatcher.Run();

                System.Diagnostics.Debug.WriteLine("[LOG] Dedicated hook dispatcher shut down.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FATAL_ERROR] HookThreadStart: Unhandled exception in hook thread: {ex.Message}");
                isHookRunning = false;
            }
        }

        /// <summary>
        /// Uninstalls the hook and stops the dedicated thread.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isHookRunning)
            {
                hookDispatcher?.Invoke(() =>
                {
                    if (keyboardHookHandle != IntPtr.Zero)
                    {
                        if (UnhookWindowsHookEx(keyboardHookHandle)) System.Diagnostics.Debug.WriteLine("[LOG] Keyboard hook uninstalled.");
                        keyboardHookHandle = IntPtr.Zero;
                    }

                    if (mouseHookHandle != IntPtr.Zero)
                    {
                        if (UnhookWindowsHookEx(mouseHookHandle)) System.Diagnostics.Debug.WriteLine("[LOG] Mouse hook uninstalled.");
                        mouseHookHandle = IntPtr.Zero;
                    }

                    hookDispatcher.InvokeShutdown();
                });

                if (hookThread.IsAlive)
                {
                    hookThread.Join(500);
                }

                isHookRunning = false;
            }
        }

        // --- Hook Installation & Callback Logic ---

        /// <summary>
        /// The callback function that is called by the system every time a keyboard event occurs.
        /// THIS EXECUTES ON THE DEDICATED HOOK THREAD.
        /// </summary>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && hookDispatcher != null)
            {
                bool shouldBlock = false;
                void setShouldBlock()
                {
                    shouldBlock = true;
                }

                int virtualKeyCode = 0;
                bool isDown = wParam switch
                {
                    WM_KEYDOWN or
                    WM_LBUTTONDOWN or
                    WM_RBUTTONDOWN or
                    WM_MBUTTONDOWN or
                    WM_XBUTTONDOWN or
                    WM_MOUSEWHEEL or
                    WM_MOUSEHWHEEL
                    => true,
                    _ => false,
                };

                if (wParam == WM_KEYDOWN || wParam == WM_KEYUP)
                {
                    KeyboardLowLevelHookStruct keyboardStruct = System.Runtime.InteropServices.Marshal.PtrToStructure<KeyboardLowLevelHookStruct>(lParam);
                    virtualKeyCode = keyboardStruct.VirtualKeyCode;
                }
                else if (IsMouseEvent(wParam))
                {
                    MouseLowLevelHookStruct mouseStruct = System.Runtime.InteropServices.Marshal.PtrToStructure<MouseLowLevelHookStruct>(lParam);

                    virtualKeyCode = wParam switch
                    {
                        WM_LBUTTONDOWN or WM_LBUTTONUP => VK_LBUTTON,
                        WM_RBUTTONDOWN or WM_RBUTTONUP => VK_RBUTTON,
                        WM_MBUTTONDOWN or WM_MBUTTONUP => VK_MBUTTON,
                        WM_XBUTTONDOWN or WM_XBUTTONUP => (mouseStruct.MouseData >> 16) switch
                        {
                            1 => VK_XBUTTON1,
                            2 => VK_XBUTTON2,
                            _ => 0
                        },
                        WM_MOUSEWHEEL => ((short)(mouseStruct.MouseData >> 16) > 0) ? VK_WHEEL_UP : VK_WHEEL_DOWN,
                        WM_MOUSEHWHEEL => ((short)(mouseStruct.MouseData >> 16) > 0) ? VK_WHEEL_RIGHT : VK_WHEEL_LEFT,
                        _ => 0
                    };

                }

                if (virtualKeyCode != 0)
                {
                    if (isDown)
                    {
                        switch (virtualKeyCode)
                        {
                            case (int)VirtualKeys.Keyboard.LeftControl:
                            case (int)VirtualKeys.Keyboard.RightControl:
                                modifiers.IsControlActive = true;
                                break;
                            case (int)VirtualKeys.Keyboard.LeftShift:
                            case (int)VirtualKeys.Keyboard.RightShift:
                                modifiers.IsShiftActive = true;
                                break;
                            case (int)VirtualKeys.Keyboard.LeftWindows:
                            case (int)VirtualKeys.Keyboard.RightWindows:
                                modifiers.IsWindowsActive = true;
                                break;
                        }

                        Hotkey key = new(virtualKeyCode, modifiers);

                        OnKeyDown(virtualKeyCode, setShouldBlock, key);
                    }
                    else
                    {
                        switch (virtualKeyCode)
                        {
                            case (int)VirtualKeys.Keyboard.LeftControl:
                            case (int)VirtualKeys.Keyboard.RightControl:
                                modifiers.IsControlActive = false;
                                break;
                            case (int)VirtualKeys.Keyboard.LeftShift:
                            case (int)VirtualKeys.Keyboard.RightShift:
                                modifiers.IsShiftActive = false;
                                break;
                            case (int)VirtualKeys.Keyboard.LeftWindows:
                            case (int)VirtualKeys.Keyboard.RightWindows:
                                modifiers.IsWindowsActive = false;
                                break;
                        }
                        Hotkey key = new(virtualKeyCode, modifiers);

                        OnKeyUp(virtualKeyCode, setShouldBlock, key);
                    }
                }

                if (shouldBlock)
                {
                    return new IntPtr(1);
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static bool IsMouseEvent(IntPtr wParam)
        {
            return wParam == WM_LBUTTONDOWN || wParam == WM_LBUTTONUP ||
                   wParam == WM_RBUTTONDOWN || wParam == WM_RBUTTONUP ||
                   wParam == WM_MBUTTONDOWN || wParam == WM_MBUTTONUP ||
                   wParam == WM_XBUTTONDOWN || wParam == WM_XBUTTONUP ||
                   wParam == WM_MOUSEWHEEL || wParam == WM_MOUSEHWHEEL;
        }

        /// <summary>
        /// Raises the <see cref="KeyDown"/> event.
        /// </summary>
        private void OnKeyDown(int virtualKeyCode, Action shouldBlockAction, Hotkey key)
        {
            KeyDown?.Invoke(this, new InputHookEventArgs(virtualKeyCode, shouldBlockAction, key));
        }

        /// <summary>
        /// Raises the <see cref="KeyUp"/> event.
        /// </summary>
        private void OnKeyUp(int virtualKeyCode, Action shouldBlockAction, Hotkey key)
        {
            KeyUp?.Invoke(this, new InputHookEventArgs(virtualKeyCode, shouldBlockAction, key));
        }
    }
}