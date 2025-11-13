using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using RotorisLib.UI;

namespace RotorisConfigurationTool.ConfigurationControls.RadialMenuManagement
{
    public partial class Component
    {
        private void EditTextIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox && e.NewValue is bool visible && visible)
            {
                textBox.Focus();
            }
        }
        private void EditTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox && textBox.DataContext is RadialMenuItemState ctx)
            {
                ctx.UpdateMenuName();
            }
        }

        private void EditTextLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is RadialMenuItemState ctx)
            {
                ctx.UpdateMenuName();
            }
        }
    }

    public class RadialMenuItemState : DependencyObject
    {
        public ICommand EditMenuNameCommand { get; }
        public ICommand EditMenuOptionsCommand { get; }
        public ICommand ShowContextMenuCommand { get; }
        public ICommand RemoveMenuCommand { get; }

        public readonly Window window;
        public readonly SettingsManager settings;
        public RadialMenuItemState(Window w, SettingsManager s, string name, ICommand removeMenuCommand)
        {
            EditMenuNameCommand = new RelayCommand(ExecuteEditMenuName);
            EditMenuOptionsCommand = new RelayCommand(ExecuteEditMenuOptions);
            ShowContextMenuCommand = new RelayCommand(ExecuteShowContextMenu);
            RemoveMenuCommand = removeMenuCommand;
            window = w;
            settings = s;
            MenuName = name;
        }

        private void ExecuteEditMenuName()
        {
            EditingMenuName = MenuName;
            IsEditing = true;
        }

        private void ExecuteEditMenuOptions()
        {
            if (MenuName == SettingsManager.RootRadialMenuFileName || !settings.RadialMenuNames.Contains(MenuName))
            {
                return;
            }

            var preview = new Preview.PreviewWindow(settings) { Owner = window };
            preview.RadialMenuChanged += (name, updatedOptions) => settings.UpdateRadialMenu(name, updatedOptions);
            preview.IsRadialMenuEditor(settings, MenuName);
            preview.ShowDialog();
        }

        private void ExecuteShowContextMenu(object? parameter)
        {
            if (parameter is Grid element)
            {
                ContextMenu contextMenu = element.ContextMenu;
                if (contextMenu.IsEnabled)
                {
                    contextMenu.PlacementTarget = element;
                    contextMenu.IsOpen = true;
                }
            }
        }

        public static readonly DependencyProperty MenuNameProperty =
            DependencyProperty.Register(
                nameof(MenuName),
                typeof(string),
                typeof(RadialMenuItemState),
                new PropertyMetadata("")
                );

        public string MenuName
        {
            get => (string)GetValue(MenuNameProperty);
            set => SetValue(MenuNameProperty, value);
        }

        public static readonly DependencyProperty EditingMenuNameProperty =
            DependencyProperty.Register(
                nameof(EditingMenuName),
                typeof(string),
                typeof(RadialMenuItemState),
                new PropertyMetadata("")
                );

        public string EditingMenuName
        {
            get => (string)GetValue(EditingMenuNameProperty);
            set => SetValue(EditingMenuNameProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(RadialMenuItemState),
                new PropertyMetadata(false)
                );

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }
        public void UpdateMenuName()
        {
            IsEditing = false;
            string oldValue = MenuName;
            string newValue = EditingMenuName.TrimEnd('/');

            if (!newValue.EndsWith(".json"))
            {
                newValue += ".json";
            }
            if (newValue != oldValue && SettingsManager.IsValidModuleName(newValue))
            {
                MenuName = newValue;
                settings.RenameRadialMenu(oldValue, newValue);
            }
        }
    }

    public class RadialMenuItemStateConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is Component component && component.DataContext is MainContext ctx && values[1] is string menuName && values[2] is ICommand removeMenuCommand)
            {
                Window window = Window.GetWindow(component);
                return new RadialMenuItemState(window, ctx.Settings, menuName, removeMenuCommand);
            }

            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
