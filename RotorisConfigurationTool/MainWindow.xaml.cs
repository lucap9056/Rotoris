using RotorisConfigurationTool.ConfigurationControls;
using System.Diagnostics;
using System.Windows;

namespace RotorisConfigurationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += (sender, e) =>
            {
                if (DataContext is MainContext ctx)
                {
                    ctx.InputHook.Dispose();
                }
            };
        }
    }

}