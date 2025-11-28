using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Rotoris.Logger;
using RotorisLib;

namespace Rotoris.MainViewer
{
    public partial class MainWindow
    {
        public void InitializeEventAggregatorSubscriptions()
        {
            EventAggregator.ShowMenuReceived += Dispatch<EventAggregator.ShowMenuReceiveEventArgs>((sender, e) => Show());
            EventAggregator.HideMenuReceived += Dispatch<EventArgs>((sender, e) => Hide());

            EventAggregator.UIDisplayMessageReceived += Dispatch<EventAggregator.UIDisplayMessageReceiveEventArgs>(OnDisplayMessageRequested);
            EventAggregator.UIDrawOptionsReceived += Dispatch<EventAggregator.UIDrawOptionsReceiveEventArgs>(OnDrawOptionsRequested);
            EventAggregator.UIFocusOptionReceived += Dispatch<EventAggregator.UIFocusOptionReceiveEventArgs>(OnFocusOptionRequested);
            EventAggregator.UISetSizeReceived += Dispatch<EventAggregator.UISetSizeReceiveEventArgs>(OnSetSizeRequested);
            EventAggregator.UILoadConfigurationReceived += Dispatch<EventAggregator.UILoadConfigurationReceiveEventArgs>(OnUILoadConfigurationRequested);
            EventAggregator.UIDrawMessageCanvasReceived += Dispatch<EventAggregator.UIDrawMessageCanvasReceiveEventArgs>(OnDrawMessageCanvas);
            EventAggregator.UIClearMessageCanvasReceived += Dispatch<EventArgs>((sender, e) => ClearMessageCanvas());
            EventAggregator.ExitReceived += Dispatch<EventArgs>((sender, e) => Close());
        }
        private EventHandler<T> Dispatch<T>(Action<object?, T> action)
        {
            return (sender, e) =>
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

        /*
         * 
         * 
         * 
         */

        private new void Show()
        {
            Point center = UserInterface.GetFocusedScreenCenter(this);
            Left = center.X - Width / 2;
            Top = center.Y - Height / 2;
            ClearMessageCanvas();
            Visibility = Visibility.Visible;
            Status.UpdateMenuVisibledState(true);
        }

        private new void Hide()
        {
            viewModel.MenuOptions = [];
            Visibility = Visibility.Hidden;
            Status.UpdateMenuVisibledState(false);
        }
        private void OnUILoadConfigurationRequested(object? sender, EventAggregator.UILoadConfigurationReceiveEventArgs e)
        {
            viewModel.Size = e.Configuration.UiSize;

            var configuration = e.Configuration;
            var brushes = new UserInterface.AppThemeBrushes();
            {
                if (configuration.UiBackground is Color background)
                {
                    brushes.BackgroundBrush = UserInterface.AppThemeBrushes.BrushFromColor(background);
                }

                if (configuration.UiForeground is Color foreground)
                {
                    brushes.ForegroundBrush = UserInterface.AppThemeBrushes.BrushFromColor(foreground);
                }
                if (configuration.UiAccent is Color accent)
                {
                    brushes.AccentBrush = UserInterface.AppThemeBrushes.BrushFromColor(accent);
                }

                viewModel.ThemeBrushes = brushes;
            }
        }
        public void OnSetSizeRequested(object? sender, EventAggregator.UISetSizeReceiveEventArgs e)
        {
            viewModel.Size = e.Size;
        }
        public void OnDisplayMessageRequested(object? sender, EventAggregator.UIDisplayMessageReceiveEventArgs e)
        {
            viewModel.MessageContent = e.Content ?? "";
        }

        public void OnDrawOptionsRequested(object? sender, EventAggregator.UIDrawOptionsReceiveEventArgs e)
        {
            viewModel.MenuOptions = e.Options;
            viewModel.OptionSector = new RotorisLib.UI.ViewModel.OptionSectorData(Width, Height, e.Options.Length, viewModel.Padding);
            viewModel.FocusedMenuOptionIndex = 0;
        }

        public void OnFocusOptionRequested(object? sender, EventAggregator.UIFocusOptionReceiveEventArgs e)
        {
            viewModel.FocusedMenuOptionIndex = e.Index;
        }

        /*
         * 
         * 
         * 
         */
        private readonly Lock messageCanvasLock = new();
        private void OnDrawMessageCanvas(object? sender, EventAggregator.UIDrawMessageCanvasReceiveEventArgs e)
        {
            lock (messageCanvasLock)
            {
                if (viewModel.MessageCanvasBitmap == null || viewModel.MessageCanvasBitmap.PixelWidth != e.Width || viewModel.MessageCanvasBitmap.PixelHeight != e.Height)
                {
                    viewModel.MessageCanvasBitmap = new WriteableBitmap(e.Width, e.Height, 96, 96, PixelFormats.Pbgra32, null);
                }

                viewModel.MessageCanvasBitmap.Lock();

                Marshal.Copy(e.Data, 0, viewModel.MessageCanvasBitmap.BackBuffer, e.Data.Length);

                viewModel.MessageCanvasBitmap.AddDirtyRect(new Int32Rect(0, 0, e.Width, e.Height));

                viewModel.MessageCanvasBitmap.Unlock();
            }
        }

        private void ClearMessageCanvas()
        {
            if (viewModel.MessageCanvasBitmap != null)
            {
                var bitmap = viewModel.MessageCanvasBitmap;
                int size = bitmap.PixelWidth * bitmap.PixelHeight * 4;

                byte[] emptyData = new byte[size];

                bitmap.Lock();

                Marshal.Copy(emptyData, 0, bitmap.BackBuffer, size);

                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));

                bitmap.Unlock();
            }
        }
    }
}
