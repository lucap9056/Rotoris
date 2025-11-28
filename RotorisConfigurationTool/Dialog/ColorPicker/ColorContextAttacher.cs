using System.Windows.Media;

namespace RotorisConfigurationTool.Dialog.ColorPicker
{
    public class ColorContextAttacher
    {
        public static readonly System.Windows.DependencyProperty PrimaryHueColorProperty =
            System.Windows.DependencyProperty.RegisterAttached(
                "PrimaryHueColor",
                typeof(Color),
                typeof(ColorContextAttacher),
                new System.Windows.PropertyMetadata(Colors.Red)
                );

        public static Color GetPrimaryHueColor(System.Windows.DependencyObject obj)
        {
            return (Color)obj.GetValue(PrimaryHueColorProperty);
        }

        public static void SetPrimaryHueColor(System.Windows.DependencyObject obj, Color value)
        {
            obj.SetValue(PrimaryHueColorProperty, value);
        }

        private static readonly Color DefaultFinalSelectedColor = (Color)ColorConverter.ConvertFromString("#803B3B");

        public static readonly System.Windows.DependencyProperty SelectedColorSVProperty =
            System.Windows.DependencyProperty.RegisterAttached(
                "SelectedColorSV",
                typeof(Color),
                typeof(ColorContextAttacher),
                new System.Windows.PropertyMetadata(DefaultFinalSelectedColor)
                );

        public static Color GetSelectedColorSV(System.Windows.DependencyObject obj)
        {
            return (Color)obj.GetValue(SelectedColorSVProperty);
        }

        public static void SetSelectedColorSV(System.Windows.DependencyObject obj, Color value)
        {
            obj.SetValue(SelectedColorSVProperty, value);
        }
    }
}
