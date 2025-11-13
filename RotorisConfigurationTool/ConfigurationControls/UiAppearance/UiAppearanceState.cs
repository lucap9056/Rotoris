using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using RotorisLib.UI;
using RotorisLib;
using System.Diagnostics;

namespace RotorisConfigurationTool.ConfigurationControls.UiAppearance
{
    public class UiAppearanceState : DependencyObject
    {
        public ICommand SetSizeCommand { get; }
        public ICommand SetAccentColorCommand { get; }
        public ICommand SetForegroundColorCommand { get; }
        public ICommand SetBackgroundColorCommand { get; }
        public ICommand SaveConfigurationCommand { get; }
        public ICommand SetDefaultCommand { get; }

        private readonly Window window;
        private readonly SettingsManager settings;
        public UiAppearanceState(Window w, SettingsManager s)
        {
            window = w;
            settings = s;

            SetSizeCommand = new RelayCommand(ExecuteSetSize);
            SetAccentColorCommand = new RelayCommand(ExecuteSetAccentColor);
            SetForegroundColorCommand = new RelayCommand(ExecuteSetForegroundColor);
            SetBackgroundColorCommand = new RelayCommand(ExecuteSetBackgroundColor);
            SaveConfigurationCommand = new RelayCommand(ExecuteSaveConfiguration);
            SetDefaultCommand = new RelayCommand(ExecuteSetDefault);

            Configuration configuration = settings.CurrentConfig;

            UiSize = configuration.UiSize;

            if (configuration.UiAccent is Color accent)
            {
                AccentColor = accent;
            }

            if (configuration.UiForeground is Color foreground)
            {
                ForegroundColor = foreground;
            }

            if (configuration.UiBackground is Color background)
            {
                BackgroundColor = background;
            }
        }

        private void ExecuteSetSize()
        {
            Configuration configuration = new()
            {
                UiAccent = AccentColor,
                UiForeground = ForegroundColor,
                UiBackground = BackgroundColor,
            };
            var preview = new Preview.PreviewWindow(settings) { Owner = window };
            preview.SizeChanged += (size) => UiSize = size;
            preview.IsSizeEditor(UiSize, configuration);
        }

        private void ExecuteSetAccentColor()
        {
            SelectColor(UserInterface.ThemeBrushIdentifier.Accent);
        }

        private void ExecuteSetForegroundColor()
        {
            SelectColor(UserInterface.ThemeBrushIdentifier.Foreground);
        }

        private void ExecuteSetBackgroundColor()
        {
            SelectColor(UserInterface.ThemeBrushIdentifier.Background);
        }

        private void SelectColor(UserInterface.ThemeBrushIdentifier brushIdentifier)
        {
            Configuration configuration = new()
            {
                UiAccent = AccentColor,
                UiForeground = ForegroundColor,
                UiBackground = BackgroundColor,
            };
            var preview = new Preview.PreviewWindow(settings) { Owner = window };
            preview.ColorChanged += SetColor;
            preview.IsColorEditor(brushIdentifier, configuration);
        }

        private void ExecuteSaveConfiguration()
        {
            settings.UpdateUiSize(UiSize);

            Configuration configuration = new()
            {
                UiAccent = AccentColor,
                UiForeground = ForegroundColor,
                UiBackground = BackgroundColor,
            };
            settings.UpdateUiColors(configuration);
            settings.SaveSettings();
        }

        private void ExecuteSetDefault()
        {
            UiSize = Configuration.Default.UiSize;
            BackgroundColor = Configuration.Default.UiBackground;
            ForegroundColor = Configuration.Default.UiForeground;
            AccentColor = Configuration.Default.UiAccent;
        }

        private void SetColor(UserInterface.ThemeBrushIdentifier name, Color color)
        {
            switch (name)
            {
                case UserInterface.ThemeBrushIdentifier.Background:
                    BackgroundColor = color;
                    break;
                case UserInterface.ThemeBrushIdentifier.Foreground:
                    ForegroundColor = color;
                    break;
                case UserInterface.ThemeBrushIdentifier.Accent:
                    AccentColor = color;
                    break;
            }
        }

        public static readonly DependencyProperty UiSizeProperty =
            DependencyProperty.Register(
                nameof(UiSize),
                typeof(double),
                typeof(UiAppearanceState),
                new PropertyMetadata(0.0)
                );

        public double UiSize
        {
            get => (double)GetValue(UiSizeProperty);
            set => SetValue(UiSizeProperty, value);
        }


        public static readonly DependencyProperty AccentColorProperty =
            DependencyProperty.Register(
                nameof(AccentColor),
                typeof(Color?),
                typeof(UiAppearanceState),
                new PropertyMetadata(null)
                );

        public Color? AccentColor
        {
            get => (Color?)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }



        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register(
                nameof(ForegroundColor),
                typeof(Color?),
                typeof(UiAppearanceState),
                new PropertyMetadata(null)
                );

        public Color? ForegroundColor
        {
            get => (Color?)GetValue(ForegroundColorProperty);
            set => SetValue(ForegroundColorProperty, value);
        }



        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(
                nameof(BackgroundColor),
                typeof(Color?),
                typeof(UiAppearanceState),
                new PropertyMetadata(null)
                );

        public Color? BackgroundColor
        {
            get => (Color?)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
    }

    public class UiAppearanceStateConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is Component component && component.DataContext is MainContext ctx && values[1] is bool isTabSelected && isTabSelected)
            {
                Window window = Window.GetWindow(component);
                return new UiAppearanceState(window, ctx.Settings);
            }

            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BrushFromColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return UserInterface.AppThemeBrushes.BrushFromColor(color);
            }

            return UserInterface.AppThemeBrushes.BrushFromColor(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
