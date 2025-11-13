
using RotorisConfigurationTool.Properties.ConfigurationControls.ActionManagement;
using RotorisConfigurationTool.Dialog;
using System.Windows.Input;
using System.Windows;
using RotorisLib.UI;

namespace RotorisConfigurationTool.ConfigurationControls.ActionManagement
{
    internal class ActionManagementState : DependencyObject
    {
        public ICommand AppendActionCommand { get; }
        public ICommand RemoveActionCommand { get; }

        private readonly SettingsManager settings;
        public ActionManagementState(SettingsManager s)
        {
            settings = s;
            AppendActionCommand = new RelayCommand(ExecuteAppendAction);
            RemoveActionCommand = new RelayCommand(ExecuteRemoveAction);

            UpdateActionList();
        }



        public static readonly DependencyProperty ActionListProperty =
            DependencyProperty.Register(
                nameof(ActionList),
                typeof(IEnumerable<string>),
                typeof(ActionManagementState),
                new PropertyMetadata((IEnumerable<string>)[])
                );

        public IEnumerable<string> ActionList
        {
            get => (IEnumerable<string>)GetValue(ActionListProperty);
            set => SetValue(ActionListProperty, value);
        }

        public static readonly DependencyProperty AppendInputValueProperty =
            DependencyProperty.Register(
                nameof(AppendInputValue),
                typeof(string),
                typeof(ActionManagementState),
                new PropertyMetadata("")
                );

        public string AppendInputValue
        {
            get => (string)GetValue(AppendInputValueProperty);
            set => SetValue(AppendInputValueProperty, value);
        }
        private void ExecuteAppendAction()
        {
            string actionName = AppendInputValue;

            if (actionName.EndsWith("/.lua", StringComparison.OrdinalIgnoreCase))
            {
                actionName = actionName[..(actionName.Length - 6)] + "/index.lua";
            }

            if (string.IsNullOrEmpty(actionName) || settings.ExternalActionNames.Contains(actionName) && !SettingsManager.IsValidModuleName(actionName))
            {
                return;
            }

            settings.UpdateExternalAction(actionName, false, "");
            UpdateActionList();
            AppendInputValue = "";
        }

        private void ExecuteRemoveAction(object? parameter)
        {
            if (parameter is string actionName)
            {
                string message = I18n.ResourceManager.GetString("RemoveAlert", SettingsManager.CurrentCulture) ?? "";
                if (Alert.Show(MessageBoxButton.YesNo, string.Format(message, actionName)) ?? false)
                {
                    settings.RemoveExternalAction(actionName);
                    UpdateActionList();
                }
            }
        }

        public void UpdateActionList()
        {
            ActionList = settings.ExternalActionNames;
        }

    }

    public class ActionManagementStateConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Component component && component.DataContext is MainContext ctx)
            {
                return new ActionManagementState(ctx.Settings);
            }

            return DependencyProperty.UnsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
