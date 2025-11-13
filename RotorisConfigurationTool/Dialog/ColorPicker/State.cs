namespace RotorisConfigurationTool.Dialog.ColorPicker
{
    public delegate void ColorSelectEventHandler(System.Windows.Media.Color color);
    internal class ColorPickerState : System.Windows.DependencyObject
    {

        public static readonly System.Windows.DependencyProperty HueProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Hue),
                typeof(double),
                typeof(ColorPickerState),
                new System.Windows.PropertyMetadata(0.0, ColorComponentChanged)
                );

        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }



        public static readonly System.Windows.DependencyProperty SaturationProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Saturation),
                typeof(double),
                typeof(ColorPickerState),
                new System.Windows.PropertyMetadata(0.5, ColorComponentChanged)
                );

        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }



        public static readonly System.Windows.DependencyProperty ValueProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(ColorPickerState),
                new System.Windows.PropertyMetadata(0.5, ColorComponentChanged)
                );

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }



        public static readonly System.Windows.DependencyProperty AlphaProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Alpha),
                typeof(byte),
                typeof(ColorPickerState),
                new System.Windows.PropertyMetadata((byte)255, ColorComponentChanged)
                );

        public byte Alpha
        {
            get => (byte)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }

        public event ColorSelectEventHandler? ColorSelected;
        private static void ColorComponentChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPickerState state)
            {
                var finalSelectedColor = Hvs.ToColor(state.Hue, state.Saturation, state.Value, state.Alpha);
                state.ColorSelected?.Invoke(finalSelectedColor);
            }
        }
    }
}
