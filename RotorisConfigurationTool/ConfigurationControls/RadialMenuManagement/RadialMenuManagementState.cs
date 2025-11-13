using RotorisConfigurationTool.Dialog;
using RotorisConfigurationTool.Properties.ConfigurationControls.RadialMenuManagement;
using RotorisLib;
using RotorisLib.UI;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace RotorisConfigurationTool.ConfigurationControls.RadialMenuManagement
{
    public class RadialMenuManagementState : DependencyObject
    {
        public ICommand EditMainRadialMenuCommand { get; }
        public ICommand AppendMenuCommand { get; }
        public ICommand RemoveMenuCommand { get; }

        private readonly Window window;
        private readonly SettingsManager settings;
        public RadialMenuManagementState(Window w, SettingsManager s)
        {
            window = w;
            settings = s;
            EditMainRadialMenuCommand = new RelayCommand(ExecuteEditMainRadialMenu);
            AppendMenuCommand = new RelayCommand(ExecuteAppendRadialMenu);
            RemoveMenuCommand = new RelayCommand(ExecuteRemoveRadialMenu);
            UpdateRadialMenuList();
        }


        public static readonly DependencyProperty RadialMenuListProperty =
            DependencyProperty.Register(
                nameof(RadialMenuList),
                typeof(IEnumerable<string>),
                typeof(RadialMenuManagementState),
                new PropertyMetadata((IEnumerable<string>)[])
                );

        public IEnumerable<string> RadialMenuList
        {
            get => (IEnumerable<string>)GetValue(RadialMenuListProperty);
            set => SetValue(RadialMenuListProperty, value);
        }


        public static readonly DependencyProperty AppendInputValueProperty =
            DependencyProperty.Register(
                nameof(AppendInputValue),
                typeof(string),
                typeof(RadialMenuManagementState),
                new PropertyMetadata("")
                );

        public string AppendInputValue
        {
            get => (string)GetValue(AppendInputValueProperty);
            set => SetValue(AppendInputValueProperty, value);
        }
        private void ExecuteEditMainRadialMenu()
        {
            string menuName = SettingsManager.RootRadialMenuFileName;

            var preview = new Preview.PreviewWindow(settings) { Owner = window };
            preview.Hide();
            preview.RadialMenuChanged += OnRadialMenuChanged;
            preview.IsRadialMenuEditor(settings, menuName);
            preview.ShowDialog();
        }
        private void ExecuteAppendRadialMenu()
        {
            string menuName = AppendInputValue;
            if (menuName.EndsWith("/.json", StringComparison.OrdinalIgnoreCase))
            {
                menuName = menuName[..(menuName.Length - 6)] + "/index.json";
            }
            Debug.WriteLine($"Append: [{menuName}]");

            if (string.IsNullOrEmpty(menuName) || settings.RadialMenuNames.Contains(menuName) && !SettingsManager.IsValidModuleName(menuName))
            {
                return;
            }

            AppendInputValue = "";
            var preview = new Preview.PreviewWindow(settings) { Owner = window };
            preview.RadialMenuChanged += OnRadialMenuChanged;
            preview.IsRadialMenuEditor(settings, menuName);
            preview.ShowDialog();
        }

        private void ExecuteRemoveRadialMenu(object? parameter)
        {
            if (parameter is string menuName)
            {
                string message = I18n.ResourceManager.GetString("RemoveAlert", SettingsManager.CurrentCulture) ?? "";
                string name = SettingsManager.RemoveJsonSuffix(menuName);
                if (Alert.Show(MessageBoxButton.YesNo, string.Format(message, name)) ?? false)
                {
                    settings.RemoveRadialMenu(menuName);
                    UpdateRadialMenuList();
                }
            }
        }

        private void OnRadialMenuChanged(string name, MenuOptionData[] options)
        {
            settings.UpdateRadialMenu(name, options);
            UpdateRadialMenuList();
        }
        public void UpdateRadialMenuList()
        {
            RadialMenuList = settings.RadialMenuNames.Where((name) => name != SettingsManager.RootRadialMenuFileName);
        }
    }

    public class RadialMenuManagementStateConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Component component && component.DataContext is MainContext ctx)
            {
                Window window = Window.GetWindow(component);
                return new RadialMenuManagementState(window, ctx.Settings);
            }

            return DependencyProperty.UnsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
