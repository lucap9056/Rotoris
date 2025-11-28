using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace RotorisConfigurationTool.Dialog.ColorPicker
{
    /// <summary>
    /// ColorPicker.xaml 的互動邏輯
    /// </summary>
    public partial class PopupWindow : Window
    {
        public event ColorSelectEventHandler? ColorSelected;
        private readonly ColorPickerViewModel viewModel = new();
        public PopupWindow(Color initialColor)
        {
            InitializeComponent();
            Resources["ViewModel"] = viewModel;

            var (H, S, V) = Hvs.FromColor(initialColor);
            viewModel.Hue = H;
            viewModel.Saturation = S;
            viewModel.Value = V;

            viewModel.ColorSelected += (color) =>
            {
                ColorSelected?.Invoke(color);
            };
        }

        private void ColorSVArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid area)
            {
                ColorSVArea_Update(area, e.GetPosition(area));
                area.CaptureMouse();
            }
        }
        private void ColorSVArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Grid area && e.LeftButton == MouseButtonState.Pressed && area.IsMouseCaptured)
            {
                ColorSVArea_Update(area, e.GetPosition(area));
            }
        }
        private void ColorSVArea_Update(Grid area, Point p)
        {
            double width = area.ActualWidth;
            double height = area.ActualHeight;

            double x = Math.Max(0, Math.Min(width, p.X));
            double y = Math.Max(0, Math.Min(height, p.Y));

            viewModel.Saturation = x / width;
            viewModel.Value = 1.0 - (y / height);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ColorSVArea.IsMouseCaptured)
            {
                ColorSVArea.ReleaseMouseCapture();
            }
            base.OnMouseLeftButtonUp(e);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    BindingExpression expression = textBox.GetBindingExpression(TextBox.TextProperty);
                    expression?.UpdateSource();
                }
                e.Handled = true;
                Keyboard.ClearFocus();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }


}
