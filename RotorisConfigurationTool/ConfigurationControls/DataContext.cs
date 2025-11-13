using RotorisLib;

namespace RotorisConfigurationTool.ConfigurationControls
{
    public class MainContext : System.Windows.DependencyObject
    {
        public static readonly System.Windows.DependencyProperty SettingsProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Settings),
                typeof(SettingsManager),
                typeof(MainContext),
                new System.Windows.PropertyMetadata(new SettingsManager())
                );

        public SettingsManager Settings
        {
            get => (SettingsManager)GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public static readonly System.Windows.DependencyProperty InputHookProperty =
            System.Windows.DependencyProperty.Register(
                nameof(InputHook),
                typeof(GlobalInputHook),
                typeof(MainContext),
                new System.Windows.PropertyMetadata(new GlobalInputHook())
                );

        public GlobalInputHook InputHook
        {
            get => (GlobalInputHook)GetValue(InputHookProperty);
            set => SetValue(InputHookProperty, value);
        }

        public static readonly System.Windows.DependencyProperty AvailableEditorsProperty =
            System.Windows.DependencyProperty.Register(
                nameof(AvailableEditors),
                typeof(ActionManagement.EditorAvailability),
                typeof(MainContext),
                new System.Windows.PropertyMetadata(new ActionManagement.EditorAvailability())
                );

        public ActionManagement.EditorAvailability AvailableEditors
        {
            get => (ActionManagement.EditorAvailability)GetValue(AvailableEditorsProperty);
            set => SetValue(AvailableEditorsProperty, value);
        }
    }
}
