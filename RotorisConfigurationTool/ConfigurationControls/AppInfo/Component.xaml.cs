using System.Windows.Controls;
using RotorisLib.UI;
using RotorisLib;

namespace RotorisConfigurationTool.ConfigurationControls.AppInfo
{
    /// <summary>
    /// Component.xaml 的互動邏輯
    /// </summary>
    public partial class Component : UserControl
    {
        public Component()
        {
            InitializeComponent();
        }
    }

    public class AppInfo : System.Windows.DependencyObject
    {
        public System.Windows.Input.ICommand OpenAppDataDirectoryCommand { get; }
        public AppInfo()
        {
            OpenAppDataDirectoryCommand = new RelayCommand(ExecuteOpenAppDataDirectory);

            string infoFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ini");
            var appInfo = new IniFile(infoFilePath);
            AppVersion = appInfo.ReadValue("Version", "AppVersion", "Unknown");
        }

        private void ExecuteOpenAppDataDirectory()
        {
            System.IO.Directory.CreateDirectory(AppConstants.AppDataDirectory);
            System.Diagnostics.Process.Start("explorer.exe", AppConstants.AppDataDirectory);
        }

        public static readonly System.Windows.DependencyProperty AppVersionProperty =
            System.Windows.DependencyProperty.Register(
                nameof(AppVersion),
                typeof(string),
                typeof(AppInfo),
                new System.Windows.PropertyMetadata("unknown")
                );

        public string AppVersion
        {
            get => (string)GetValue(AppVersionProperty);
            set => SetValue(AppVersionProperty, value);
        }
    }
}
