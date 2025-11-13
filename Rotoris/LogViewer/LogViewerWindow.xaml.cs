using System.Windows;
using Rotoris.Logger;

namespace Rotoris.LogViewer
{
    /// <summary>
    /// ConsoleWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LogViewerWindow : Window
    {
        private static LogViewerWindow? logViewerWindow = null;
        public static void OpenViewer()
        {
            if (logViewerWindow == null)
            {
                logViewerWindow = new LogViewerWindow();
                logViewerWindow.Closed += (_, args) =>
                {
                    logViewerWindow = null;
                };
            }
            else
            {
                logViewerWindow.Activate();
            }
        }
        public static void CloseViewer()
        {
            logViewerWindow?.Close();
            logViewerWindow = null;
        }

        private readonly EventHandler<EventAggregator.WriteLogsEventArgs> onWriteLogsHandler;
        private readonly EventHandler<EventArgs> onExitHandler;
        public LogViewerWindow()
        {
            InitializeComponent();
            onWriteLogsHandler = Dispatch<EventAggregator.WriteLogsEventArgs>(OnWriteLogs);
            onExitHandler = Dispatch<EventArgs>((sender, e) => EventAggregator.PublishHideLogs());

            EventAggregator.WriteLogsReceived += onWriteLogsHandler;
            EventAggregator.ExitReceived += onExitHandler;
            Logs.AppendText(Log.LogBuffer.ToString() + "\n");
            Logs.ScrollToEnd();
        }
        private EventHandler<T> Dispatch<T>(Action<object?, T> action)
        {
            return (object? sender, T e) =>
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => action(sender, e));
                }
                else
                {
                    action(sender, e);
                }
            };
        }
        protected override void OnClosed(EventArgs e)
        {
            EventAggregator.WriteLogsReceived -= onWriteLogsHandler;
            EventAggregator.ExitReceived -= onExitHandler;
            base.OnClosed(e);
        }

        private void OnWriteLogs(object? sender, EventAggregator.WriteLogsEventArgs e)
        {
            Logs.AppendText(e.Value);
            Logs.ScrollToEnd();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            EventAggregator.PublishExit();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            EventAggregator.PublishReload();
        }

    }
}
