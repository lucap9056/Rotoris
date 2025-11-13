using RotorisConfigurationTool.Properties.PreviewWindow;
using System.Windows.Media;
using System.Windows;
using RotorisLib;

namespace RotorisConfigurationTool.Preview
{
    /// <summary>
    /// PreviewWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public delegate void SizeChangeEventHandler(double size);
        public delegate void ColorChangeEventHandler(UserInterface.ThemeBrushIdentifier name, Color color);
        public delegate void RadialMenuChangeEventHandler(string name, MenuOptionData[] options);

        public new event SizeChangeEventHandler? SizeChanged;
        public event ColorChangeEventHandler? ColorChanged;
        public event RadialMenuChangeEventHandler? RadialMenuChanged;
        private readonly PreviewState State = new();
        public PreviewWindow(SettingsManager settings)
        {
            InitializeComponent();
            Resources["State"] = State;
            State.SizeChanged += OnSizeChanged;
            State.Settings = settings;

            var configuration = settings.CurrentConfig;
            State.Size = configuration.UiSize;
            LoadThemeBrushes(configuration);
        }
        private void LoadThemeBrushes(RotorisLib.Configuration configuration)
        {
            var themeBrushes = new UserInterface.AppThemeBrushes();
            {
                if (configuration.UiBackground is Color background)
                {
                    themeBrushes.BackgroundBrush = UserInterface.AppThemeBrushes.BrushFromColor(background);
                }

                if (configuration.UiForeground is Color foreground)
                {
                    themeBrushes.ForegroundBrush = UserInterface.AppThemeBrushes.BrushFromColor(foreground);
                }
                if (configuration.UiAccent is Color accent)
                {
                    themeBrushes.AccentBrush = UserInterface.AppThemeBrushes.BrushFromColor(accent);
                }

                State.ThemeBrushes = themeBrushes;
            }
        }

        private void OnSizeChanged(object sender, double oldValue, double newValue)
        {
            Point center = new(Width / 2 + Left, Height / 2 + Top);

            Width = newValue + State.Padding;
            Height = newValue + State.Padding;
            Left = center.X - Width / 2;
            Top = center.Y - Height / 2;
        }

        public void RadialMenuChange(string menuName, MenuOptionData[] options)
        {
            RadialMenuChanged?.Invoke(menuName, options);
        }
    }
    public class NotNullToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null)
            {
                return Visibility.Collapsed;
            }
            if (value is string s && string.IsNullOrEmpty(s))
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class MenuNameConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string name)
            {
                if (name == SettingsManager.RootRadialMenuFileName)
                {
                    return I18n.ResourceManager.GetString("RootMenu", culture) ?? name;
                }
                else
                {
                    return SettingsManager.RemoveJsonSuffix(name);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
