using System.Windows.Controls;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace RotorisConfigurationTool.ConfigurationControls.Startup
{
    /// <summary>
    /// Component.xaml 的互動邏輯
    /// </summary>
    public partial class Component : UserControl
    {
        private static readonly string AppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rotoris.exe");
        private static readonly string SubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string KeyName = RotorisLib.AppConstants.AppName;


        public static readonly DependencyProperty StartupEnabledProperty =
            DependencyProperty.Register(
                nameof(StartupEnabled),
                typeof(bool),
                typeof(Component),
                new PropertyMetadata(false, OnStartupChanged));

        public bool StartupEnabled
        {
            get { return (bool)GetValue(StartupEnabledProperty); }
            set { SetValue(StartupEnabledProperty, value); }
        }
        public Component()
        {
            InitializeComponent();
            RegistryKey? RKey = Registry.CurrentUser.OpenSubKey(SubKey);

            if (RKey != null && RKey.GetValue(KeyName) is string value && value == AppPath)
            {
                StartupEnabled = true;
            }
        }
        private static void OnStartupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool enable)
            {
                if (!File.Exists(AppPath))
                {
                    return;
                }
                RegistryKey RKey = Registry.CurrentUser.CreateSubKey(SubKey);
                if (enable) RKey.SetValue(KeyName, AppPath);
                else if (RKey.GetValue(KeyName) != null)
                {
                    RKey.DeleteValue(KeyName);
                }
            }

        }
    }
}
