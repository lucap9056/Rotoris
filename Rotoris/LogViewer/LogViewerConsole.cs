using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Rotoris.Logger;
using Rotoris.Properties.Window;

namespace Rotoris.LogViewer
{
    public static class LogViewerConsole
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private delegate bool ConsoleCtrlHandlerRoutine(uint dwCtrlType);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerRoutine handler, [MarshalAs(UnmanagedType.Bool)] bool add);

        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint CTRL_CLOSE_EVENT = 2;

        private static readonly ConsoleCtrlHandlerRoutine ctrlHandlerRoutine = OnConsoleCtrlEvent;
        private static bool ctrlHandlerInstalled;
        private static volatile bool running;
        private static Thread? keyListenerThread;

        static LogViewerConsole()
        {
            EventAggregator.ExitReceived += (_, _) => CloseViewer();
        }

        public static void OpenViewer()
        {
            if (running)
            {
                SetForegroundWindow(GetConsoleWindow());
                return;
            }

            if (GetConsoleWindow() == IntPtr.Zero)
            {
                AllocConsole();
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true });
            Console.SetIn(new StreamReader(Console.OpenStandardInput(), Encoding.UTF8));
            Console.CancelKeyPress += OnCancelKeyPress;

            if (!ctrlHandlerInstalled)
            {
                SetConsoleCtrlHandler(ctrlHandlerRoutine, true);
                ctrlHandlerInstalled = true;
            }

            EnableVirtualTerminalProcessing();
            DrawHeader();
            Console.Write(Log.LogBuffer.ToString());

            running = true;
            keyListenerThread = new Thread(KeyListenerLoop)
            {
                IsBackground = true,
                Name = "LogViewerConsoleKeyListener"
            };
            keyListenerThread.Start();
        }

        public static void CloseViewer()
        {
            if (!running)
            {
                return;
            }

            running = false;
            Console.CancelKeyPress -= OnCancelKeyPress;
            Console.Write("\x1b[r");
            FreeConsole();
        }

        private static void KeyListenerLoop()
        {
            while (running)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(50);
                    continue;
                }

                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.X:
                        EventAggregator.PublishExit();
                        break;
                    case ConsoleKey.R:
                        EventAggregator.PublishReload();
                        break;
                }
            }
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            EventAggregator.PublishHideLogs();
        }

        private static bool OnConsoleCtrlEvent(uint ctrlType)
        {
            if (ctrlType != CTRL_CLOSE_EVENT)
            {
                return false;
            }

            EventAggregator.PublishHideLogs();
            return true;
        }

        private static void EnableVirtualTerminalProcessing()
        {
            IntPtr stdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (GetConsoleMode(stdOut, out uint mode))
            {
                SetConsoleMode(stdOut, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }
        }

        private static void DrawHeader()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            string header = $"[X] {I18n.Exit}    [R] {I18n.Reload}";
            string paddedHeader = header.Length >= width ? header[..width] : header.PadRight(width);

            Console.Write(
                "\x1b[1;1H" +
                "\x1b[7m" +
                paddedHeader +
                "\x1b[0m" +
                $"\x1b[2;{height}r" +
                "\x1b[2;1H"
            );
        }
    }
}
