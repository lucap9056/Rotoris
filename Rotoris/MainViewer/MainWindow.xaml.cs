using RotorisLib;
using System.Windows;
using System.Windows.Interop;

namespace Rotoris.MainViewer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class Status
        {
            public static bool IsMenuVisibled { get; set; } = false;
            public static void UpdateMenuVisibledState(bool v)
            {
                IsMenuVisibled = v;
            }
        }

        private readonly RotorisLib.UI.State State = new();
        public MainWindow()
        {
            InitializeComponent();
            Resources["State"] = State;
            InitializeStateSubscriptions();
            InitializeEventAggregatorSubscriptions();
            SourceInitialized += MainWindowSourceInitialized;
        }
        /*
         * 
         * 
         * 
         */
        private void OnSizeChanged(object sender, double oldValue, double newValue)
        {
            double size = newValue;
            bool revisible = Visibility == Visibility.Visible;
            try
            {
                Visibility = Visibility.Hidden;

                Point center = new(Width / 2 + Left, Height / 2 + Top);

                Width = size + State.Padding;
                Height = size + State.Padding;
                Left = center.X - Width / 2;
                Top = center.Y - Height / 2;
            }
            finally
            {
                if (revisible)
                {
                    Visibility = Visibility.Visible;
                }
            }
        }

        /*
         * 
         * 
         * 
         */
        private void MainWindowSourceInitialized(object? sender, EventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;

            IntPtr exStyle = User32.GetWindowLong(hWnd, User32.GWL_EXSTYLE);

            exStyle = new IntPtr(exStyle | User32.WS_EX_TRANSPARENT | User32.WS_EX_TOOLWINDOW);

            User32.SetWindowLong(hWnd, User32.GWL_EXSTYLE, exStyle);
        }
    }
}
