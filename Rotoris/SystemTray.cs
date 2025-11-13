using System.Windows.Forms;
using Rotoris.Properties.Window;

namespace Rotoris
{
    internal class SystemTray : IDisposable
    {
        private readonly NotifyIcon notifyIcon;
        public SystemTray()
        {
            ContextMenuStrip contextMenu = new();
            contextMenu.Items.Add(I18nText("ShowLogs"), null, (sender, e) => EventAggregator.PublishShowLogs());
            contextMenu.Items.Add(I18nText("Reload"), null, (sender, e) => EventAggregator.PublishReload());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(I18nText("Exit"), null, (sender, e) => EventAggregator.PublishExit());

            NotifyIcon notify = new()
            {
                Text = RotorisLib.AppConstants.AppName,
                Icon = Properties.Resources.favicon,
                ContextMenuStrip = contextMenu,
                Visible = true
            };

            notify.Click += (sender, e) =>
            {
                if (e is MouseEventArgs mouseEventArgs && mouseEventArgs.Button == MouseButtons.Left)
                {
                    EventAggregator.PublishShowLogs();
                }
            };

            notifyIcon = notify;
        }

        private static string I18nText(string key)
        {
            return I18n.ResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
        }
        public void Dispose()
        {
            notifyIcon.Dispose();
        }
    }
}
