
using System.Windows.Controls;

namespace RotorisConfigurationTool.ConfigurationControls.ActionManagement
{
    /// <summary>
    /// ActionManagement.xaml 的互動邏輯
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
        private void AppendActionKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && sender is TextBox textBox && textBox.DataContext is ActionManagementViewModel ctx)
            {
                if (ctx.AppendActionCommand.CanExecute(textBox))
                {
                    ctx.AppendActionCommand.Execute(null);
                }
                e.Handled = true;
            }
        }
    }
}
