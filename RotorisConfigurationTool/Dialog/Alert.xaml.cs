using System.Windows;
using System.Windows.Controls;

namespace RotorisConfigurationTool.Dialog
{
    /// <summary>
    /// Alert.xaml 的互動邏輯
    /// </summary>W
    public partial class Alert : Window
    {
        private Alert(MessageBoxButton buttons, string text, string title = "")
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(text))
            {
                Title = title;
            }

            MessageText.Text = text;

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    OkButton.IsDefault = true;
                    break;
                case MessageBoxButton.OKCancel:
                    OkButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    YesButton.IsDefault = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    YesButton.IsDefault = true;
                    break;
            }
        }

        public static bool? Show(MessageBoxButton buttons, string text, string title = "")
        {
            Alert alert = new(buttons, text, title);
            return alert.ShowDialog();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case nameof(OkButton):
                        DialogResult = true;
                        break;
                    case nameof(CancelButton):
                        DialogResult = null;
                        break;
                    case nameof(YesButton):
                        DialogResult = true;
                        break;
                    case nameof(NoButton):
                        DialogResult = false;
                        break;
                }
            }
            Close();
        }
    }
}
