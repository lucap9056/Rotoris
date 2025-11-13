using System.Windows;

namespace RotorisConfigurationTool.Dialog.SizeSlider
{
    /// <summary>
    /// SizeSlider.xaml 的互動邏輯
    /// </summary>
    public delegate void SizeChangeEventHandler(double size);
    public partial class PopupWindow : Window
    {

        public new event SizeChangeEventHandler? SizeChanged;

        public static readonly DependencyProperty UiSizeProperty =
            DependencyProperty.Register(
                nameof(UiSize),
                typeof(double),
                typeof(PopupWindow),
                new PropertyMetadata(400.0, OnSizeChanged));

        public double UiSize
        {
            get { return (double)GetValue(UiSizeProperty); }
            set { SetValue(UiSizeProperty, value); }
        }
        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PopupWindow silder)
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;
                if (oldValue == newValue)
                {
                    return;
                }

                silder.OnSizeValueChanged(newValue);
            }
        }

        public PopupWindow(double value)
        {
            InitializeComponent();
            UiSize = value;
        }

        public void OnSizeValueChanged(double value)
        {
            SizeChanged?.Invoke(value);
        }


        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
