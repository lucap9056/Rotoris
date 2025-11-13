using RotorisConfigurationTool.Properties.PreviewWindow;
using RotorisConfigurationTool.Dialog;
using Newtonsoft.Json;
using RotorisLib.UI;
using RotorisLib;

namespace RotorisConfigurationTool.Preview
{
    public class PreviewState : State
    {
        public System.Windows.Input.ICommand SelectOptionCommand { get; }
        public System.Windows.Input.ICommand EditOptionCommand { get; }
        public System.Windows.Input.ICommand AppendOptionCommand { get; }
        public System.Windows.Input.ICommand CloseCommand { get; }
        public PreviewState()
        {
            SelectOptionCommand = new RelayCommand(ExecuteSelectOption);
            EditOptionCommand = new RelayCommand(ExecuteEditOption);
            AppendOptionCommand = new RelayCommand(ExecuteAppendOption);
            CloseCommand = new RelayCommand(ExecuteClose);
        }
        private void ExecuteSelectOption(object? parameter)
        {
            if (parameter is int index && index != 0)
            {
                FocusedMenuOptionIndex = index;
            }
        }

        private void ExecuteEditOption(object? parameter)
        {
            if (parameter is PreviewWindow window)
            {
                List<MenuOptionData> options = [.. MenuOptions];
                MenuOptionData option = options[FocusedMenuOptionIndex];


                Dialog.OptionEditor.PopupWindow editor = new(Settings, option) { Owner = window };
                editor.OptionUpdated += (updatedOption) =>
                {
                    option = updatedOption;
                };

                if (editor.ShowDialog() ?? false)
                {

                    if (string.IsNullOrEmpty(option.Id))
                    {
                        if (options.Count == 2)
                        {
                            FocusedMenuOptionIndex = 1;
                            options = [AppConstants.BuiltInOptions.Close, AppConstants.BuiltInOptions.Empty];
                        }
                        else
                        {
                            options.RemoveAt(FocusedMenuOptionIndex);
                            if (FocusedMenuOptionIndex != 1)
                            {
                                FocusedMenuOptionIndex--;
                            }
                        }
                    }
                    else
                    {
                        options[FocusedMenuOptionIndex] = option;
                    }
                    MenuOptions = [.. options];
                    OptionSector = new OptionSectorData(window.Width, window.Height, options.Count, Padding);
                }

            }
        }

        private void ExecuteAppendOption(object? parameter)
        {
            if (parameter is PreviewWindow window)
            {
                MenuOptionData option = AppConstants.BuiltInOptions.Empty;

                Dialog.OptionEditor.PopupWindow editor = new(Settings, option) { Owner = window };
                editor.OptionUpdated += (updatedOption) =>
                {
                    option = updatedOption;
                };

                if (editor.ShowDialog() ?? false && !string.IsNullOrEmpty(option.Id))
                {
                    MenuOptions = [.. MenuOptions, option];
                    OptionSector = new OptionSectorData(window.Width, window.Height, MenuOptions.Length, Padding);
                    FocusedMenuOptionIndex = MenuOptions.Length - 1;
                }

            }
        }

        private void ExecuteClose(object? parameter)
        {
            if (parameter is PreviewWindow window)
            {
                string updateMenuOptionsString = JsonConvert.SerializeObject(MenuOptions, Formatting.Indented);
                if (OriginalMenuOptionsString == updateMenuOptionsString)
                {
                    window.DialogResult = false;
                    window.Close();
                    return;
                }

                string message = I18n.ResourceManager.GetString("SaveMenuChangesAlert", SettingsManager.CurrentCulture) ?? "";
                if (Alert.Show(System.Windows.MessageBoxButton.YesNo, message) ?? false)
                {
                    MenuOptionData[] options = MenuOptions[1..];

                    window.RadialMenuChange(MenuName, options);
                    window.DialogResult = true;
                    window.Close();
                }
                else
                {
                    window.DialogResult = false;
                    window.Close();
                }
            }
        }

        /*
         * 
         * 
         * 
         */

        public static readonly System.Windows.DependencyProperty SettingsManagerProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Settings),
                typeof(SettingsManager),
                typeof(PreviewState),
                new System.Windows.PropertyMetadata(null)
                );

        public SettingsManager Settings
        {
            get => (SettingsManager)GetValue(SettingsManagerProperty);
            set => SetValue(SettingsManagerProperty, value);
        }

        /*
         * 
         * 
         * 
         */

        public static readonly System.Windows.DependencyProperty MenuNameProperty =
            System.Windows.DependencyProperty.Register(
                nameof(MenuName),
                typeof(string),
                typeof(PreviewState),
                new System.Windows.PropertyMetadata(null)
                );

        public string MenuName
        {
            get => (string)GetValue(MenuNameProperty);
            set => SetValue(MenuNameProperty, value);
        }

        /*
         * 
         * 
         * 
         */

        public static readonly System.Windows.DependencyProperty OriginalMenuOptionsStringProperty =
            System.Windows.DependencyProperty.Register(
                nameof(OriginalMenuOptionsString),
                typeof(string),
                typeof(PreviewState),
                new System.Windows.PropertyMetadata("")
                );

        public string? OriginalMenuOptionsString
        {
            get => (string)GetValue(OriginalMenuOptionsStringProperty);
            set => SetValue(OriginalMenuOptionsStringProperty, value);
        }
    }
}
