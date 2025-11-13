using Microsoft.Toolkit.Uwp.Notifications;
using Rotoris.Logger;
using Rotoris.LogViewer;
using Rotoris.MainViewer;
using RotorisLib;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace Rotoris
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();



        private static readonly Mutex mutex = new(true, "RotorisMutex", out isNewInstance);
        private GlobalInputHook? keyboardHook;
        private OptionManager? optionManager;
        private SystemTray? systemTray;
        private static bool isNewInstance;
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Contains("--dev"))
            {
                if (GetConsoleWindow() == IntPtr.Zero)
                {
                    AllocConsole();
                }

                Console.WriteLine("Copying embedded Lua type definition files...");
                try
                {
                    CopyEmbeddedLuaTypeFiles();
                    Console.WriteLine("Successfully copied Lua type definition files.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to copy files. {ex.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("==================================================");
                Console.WriteLine("File copying complete. Press any key to close this window...");
                Console.WriteLine("==================================================");

                Console.ReadKey();

                Shutdown();
                return;
            }

            if (!isNewInstance)
            {
                Shutdown();
                return;
            }

            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {

            };

            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                if (e.ExceptionObject is Exception unhandledException)
                {
                    Log.Error($"An unhandled exception occurred. IsTerminating: {e.IsTerminating}. Exception: {unhandledException.GetType().FullName}. Message: {unhandledException.Message}. StackTrace: {unhandledException.StackTrace}");
                }
                else
                {
                    Log.Error("An unhandled exception occurred, but the exception object was null or not an Exception type.");
                }
                Log.ExportToFile();
            };

            base.OnStartup(e);

            keyboardHook = new GlobalInputHook();
            Current.MainWindow = new MainWindow();
            optionManager = new OptionManager(keyboardHook);
            systemTray = new SystemTray();

#if DEBUG
            LogViewerWindow.OpenViewer();
#endif

            EventAggregator.ShowLogsReceived += (sender, e) =>
            {
                LogViewerWindow.OpenViewer();
            };

            EventAggregator.HideLogsReceived += (sender, e) =>
            {
                LogViewerWindow.CloseViewer();
            };

            SendStartupToast();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            keyboardHook?.Dispose();
            optionManager?.Dispose();
            systemTray?.Dispose();
            mutex.Dispose();
            base.OnExit(e);
        }
        private static void CopyEmbeddedLuaTypeFiles()
        {
            string targetDirectory = Path.Combine(AppConstants.AppDataDirectory, ".types");
            try
            {
                Directory.CreateDirectory(targetDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to create target directory '{targetDirectory}'. {ex.Message}");
                return;
            }

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string[] allResourceNames = currentAssembly.GetManifestResourceNames();

            const string resourcePrefix = "Rotoris.Resources.lua_types.";
            int prefixLength = resourcePrefix.Length;

            foreach (string resourceName in allResourceNames)
            {
                if (!resourceName.StartsWith(resourcePrefix))
                {
                    continue;
                }

                using Stream? resourceStream = currentAssembly.GetManifestResourceStream(resourceName);
                if (resourceStream == null)
                {
                    Console.WriteLine($"[Warning] Failed to get stream for resource '{resourceName}'. Skipping.");
                    continue;
                }

                try
                {
                    string fileName = resourceName[prefixLength..];
                    string destinationFilePath = Path.Combine(targetDirectory, fileName);
                    using FileStream fileStream = new(destinationFilePath, FileMode.Create, FileAccess.Write);
                    resourceStream.CopyTo(fileStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to copy resource '{resourceName}'. {ex.Message}");
                }
            }
        }

        private void SendStartupToast()
        {
            new ToastContentBuilder()
                .AddText("Rotoris is Running")
                .AddText("Rotoris has started successfully and is running in the background.")
                .Show();
        }
    }
}
