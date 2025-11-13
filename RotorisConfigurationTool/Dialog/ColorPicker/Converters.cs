using System.Diagnostics;
using System.Windows.Media;

namespace RotorisConfigurationTool.Dialog.ColorPicker
{
    public class SaturationPositionConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double width && values[1] is double saturation)
            {
                if (width > 0)
                {
                    return saturation * width - 7;
                }
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValuePositionConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double height && values[1] is double value)
            {
                if (height > 0)
                {
                    return (1.0 - value) * height - 7;
                }
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PrimaryHueColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double hue)
            {
                return Hvs.ToColor(hue);
            }

            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }

    public class SelectedColorSVConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is double hue && values[1] is double saturation && values[2] is double value)
            {
                return Hvs.ToColor(hue, saturation, value);
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FinalSelectedColorConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 4 && values[0] is double hue && values[1] is double saturation && values[2] is double value && values[3] is byte alpha)
            {
                return Hvs.ToColor(hue, saturation, value, alpha);
            }

            return Colors.Transparent;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetTypes.Length == 4 && value is Color color)
            {
                var (H, S, V) = Hvs.FromColor(color);
                return [H, S, V, color.A];
            }

            return [
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue
                ];
        }
    }
    public class HvsToHexConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 4 && values[0] is double hue && values[1] is double saturation && values[2] is double value && values[3] is byte alpha)
            {
                return Hvs.ToColor(hue, saturation, value, alpha).ToString();
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetTypes.Length == 4 && value is string colorHexString)
            {
                try
                {
                    Color convertedColor = (Color)ColorConverter.ConvertFromString(colorHexString);
                    var (H, S, V) = Hvs.FromColor(convertedColor);
                    return [H, S, V, convertedColor.A];
                }
                catch (FormatException fEx)
                {
                    Debug.WriteLine($"[Error] Failed to convert hex string '{colorHexString}' to Color. Input format is invalid. Exception: {fEx.Message}");
                }
                catch (Exception generalEx)
                {
                    Debug.WriteLine($"[Error] An unexpected error occurred during color conversion for hex string '{colorHexString}'. Exception: {generalEx}");
                }

            }

            return [
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue
                ];
        }
    }
    public class HvsToArgbConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 4 && values[0] is double hue && values[1] is double saturation && values[2] is double value && values[3] is byte alpha)
            {
                Color color = Hvs.ToColor(hue, saturation, value, alpha);
                return $"{color.A}, {color.R}, {color.G}, {color.B}";
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string argbString && !string.IsNullOrWhiteSpace(argbString))
            {
                string[] components = argbString.Split(',');

                if (components.Length == 4)
                {
                    try
                    {
                        byte a = byte.Parse(components[0].Trim());
                        byte r = byte.Parse(components[1].Trim());
                        byte g = byte.Parse(components[2].Trim());
                        byte b = byte.Parse(components[3].Trim());

                        Color color = Color.FromArgb(a, r, g, b);
                        var (H, S, V) = Hvs.FromColor(color);
                        return [H, S, V, color.A];
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"[Error] Failed to parse ARGB components from string: '{argbString}'. Details: {ex.Message}");
                    }
                    catch (OverflowException ex)
                    {
                        Console.WriteLine($"[Error] ARGB component value out of range (0-255). String: '{argbString}'. Details: {ex.Message}");
                    }
                }

            }

            return [
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue,
                System.Windows.DependencyProperty.UnsetValue
                ];
        }
    }
    public class ColorToHexConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToString();
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string colorHexString)
            {
                try
                {
                    Color convertedColor = (Color)ColorConverter.ConvertFromString(colorHexString);
                    return convertedColor;
                }
                catch (FormatException fEx)
                {
                    Debug.WriteLine($"[Error] Failed to convert hex string '{colorHexString}' to Color. Input format is invalid. Exception: {fEx.Message}");
                }
                catch (Exception generalEx)
                {
                    Debug.WriteLine($"[Error] An unexpected error occurred during color conversion for hex string '{colorHexString}'. Exception: {generalEx}");
                }
            }
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
    public class ColorToArgbConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return $"{color.A}, {color.R}, {color.G}, {color.B}";
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string argbString && !string.IsNullOrWhiteSpace(argbString))
            {
                string[] components = argbString.Split(',');

                if (components.Length == 4)
                {
                    try
                    {
                        byte a = byte.Parse(components[0].Trim());
                        byte r = byte.Parse(components[1].Trim());
                        byte g = byte.Parse(components[2].Trim());
                        byte b = byte.Parse(components[3].Trim());

                        Color resultColor = Color.FromArgb(a, r, g, b);
                        return resultColor;
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"[Error] Failed to parse ARGB components from string: '{argbString}'. Details: {ex.Message}");
                    }
                    catch (OverflowException ex)
                    {
                        Console.WriteLine($"[Error] ARGB component value out of range (0-255). String: '{argbString}'. Details: {ex.Message}");
                    }
                }

            }

            Console.WriteLine($"[Error] Input value for ConvertBack is not a valid string. Value type: {value?.GetType().FullName ?? "null"}");
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }

    class Hvs
    {
        public static Color ToColor(double h = 1.0, double s = 1.0, double v = 1.0, byte a = 255)
        {
            double r, g, b;

            if (s == 0)
            {
                r = g = b = v;
            }
            else
            {
                h = h == 360 ? 0 : h;
                h /= 60;
                int i = (int)Math.Floor(h);
                double f = h - i;
                double p = v * (1 - s);
                double q = v * (1 - s * f);
                double t = v * (1 - s * (1 - f));

                switch (i)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    default: r = v; g = p; b = q; break;
                }
            }

            return Color.FromArgb(a,
                (byte)Math.Round(r * 255),
                (byte)Math.Round(g * 255),
                (byte)Math.Round(b * 255));
        }

        public static (double H, double S, double V) FromColor(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h, s, v = max;

            if (max == 0)
            {
                s = 0;
                h = 0;
            }
            else
            {
                s = delta / max;

                if (delta == 0)
                {
                    h = 0;
                }
                else if (r == max)
                {
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    h = 2 + (b - r) / delta;
                }
                else // b == max
                {
                    h = 4 + (r - g) / delta;
                }

                h *= 60;
                if (h < 0)
                    h += 360;
            }

            return (h, s, v);
        }
    }
}
