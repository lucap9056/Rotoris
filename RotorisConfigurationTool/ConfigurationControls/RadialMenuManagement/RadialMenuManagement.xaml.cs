using System.Windows.Controls;
using System.Windows;

namespace RotorisConfigurationTool.ConfigurationControls.RadialMenuManagement
{
    /// <summary>
    /// RadialMenuManagement.xaml 的互動邏輯
    /// </summary>
    public partial class Component : UserControl
    {
        public Component()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }

        private void AppendMenuKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && sender is TextBox textBox && textBox.DataContext is RadialMenuManagementState ctx)
            {
                if (ctx.AppendMenuCommand.CanExecute(textBox))
                {
                    ctx.AppendMenuCommand.Execute(null);
                }
                e.Handled = true;
            }
        }
    }
}
