namespace RotorisLib.UI
{
    public class BooleanToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if ((parameter?.ToString() ?? "").Equals("Reverse", StringComparison.CurrentCultureIgnoreCase))
                {
                    return boolValue ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                }
                else
                {
                    return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
            else if (value != null)
            {
                return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
    public class OptionsVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int count && count > 1)
            {
                return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
    public class RenderContextInputConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is MenuOptionData[] options && options.Length > 0 && values[1] is double width && values[2] is double height)
            {
                int count = options.Length;

                double angleIncrement = 360.0 / count;
                double radius = width * 0.365;
                System.Windows.Point centerPoint = new(width / 2, height / 2);
                double iconSize = width * 0.18;
                RenderContextAttacher.RenderContextData contextData = new(angleIncrement, radius, centerPoint, iconSize);
                return contextData;
            }

            return new RenderContextAttacher.RenderContextData(0.0, 0.0, new System.Windows.Point(0, 0), 0.0);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RenderIconContextInputConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is RenderContextAttacher.RenderContextData context && values[1] is int index)
            {
                double angle = index * context.AngleIncrement - 90;
                double angleRad = angle * System.Math.PI / 180.0;

                double centerX = context.CanvasCenter.X + context.Radius * System.Math.Cos(angleRad);
                double centerY = context.CanvasCenter.Y + context.Radius * System.Math.Sin(angleRad);
                double halfIconSize = context.IconSize / 2;
                double top = centerY - halfIconSize;
                double left = centerX - halfIconSize;

                return new RenderContextAttacher.RenderIconRect(top, left, context.IconSize, context.IconSize);
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class OptionIconVisualSourceConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MenuOptionData option)
            {
                return GetOptionIconVisualSource(option);
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
        private static System.Windows.Media.Imaging.BitmapImage GetOptionIconVisualSource(MenuOptionData option)
        {
            // 1. Check for built-in options (e.g., standard system commands)
            if (AppConstants.BuiltInOptionsMap.TryGetValue(option.Id, out MenuOptionData builtInOption) && !System.String.IsNullOrEmpty(builtInOption.InternalIconResourcePath))
            {
                try
                {
                    // Using the resolved built-in path
                    return new System.Windows.Media.Imaging.BitmapImage(new System.Uri(builtInOption.InternalIconResourcePath));
                }
                catch (System.UriFormatException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Built-in option '{option.Id}' resolved to an invalid URI: {builtInOption.InternalIconResourcePath}. Exception: {ex.Message}. Continuing to next fallback.");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Error loading built-in icon for '{option.Id}'. Exception: {ex.Message}. Continuing to next fallback.");
                }
            }

            // 2. Check for explicitly defined built-in icons (by path/key)
            if (!System.String.IsNullOrEmpty(option.InternalIconResourcePath) && AppConstants.BuiltInIconPaths.TryGetValue(option.InternalIconResourcePath, out System.String? builtInIconUri) && !System.String.IsNullOrEmpty(builtInIconUri))
            {
                try
                {
                    // Using the URI from the BuiltInIconPaths map
                    return new System.Windows.Media.Imaging.BitmapImage(new System.Uri(builtInIconUri));
                }
                catch (System.UriFormatException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Explicit built-in icon path '{option.InternalIconResourcePath}' has an invalid URI: {builtInIconUri}. Exception: {ex.Message}. Continuing to next fallback.");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Error loading explicit built-in icon for '{option.Id}'. Exception: {ex.Message}. Continuing to next fallback.");
                }
            }

            // 3. Use the custom/external IconPath provided in the option
            if (!System.String.IsNullOrEmpty(option.InternalIconResourcePath))
            {
                try
                {
                    // Using the custom path provided in the option data
                    return new System.Windows.Media.Imaging.BitmapImage(new System.Uri(option.InternalIconResourcePath));
                }
                catch (System.UriFormatException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Custom icon path '{option.InternalIconResourcePath}' for option '{option.Id}' is an invalid URI. Exception: {ex.Message}. Falling back to text rendering.");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WARNING: Error loading custom icon for option '{option.Id}'. Exception: {ex.Message}. Falling back to text rendering.");
                }
            }

            // 4. Final Fallback: Render text as an image using TextImageRenderer
            if (AppConstants.TextImageRenderer != null)
            {
                System.Diagnostics.Debug.WriteLine($"INFO: No valid icon path found for option '{option.Id}'. Generating icon from text.");
                // Note: The ToBitmapImage method should handle its own exceptions/return default on failure.
                return AppConstants.TextImageRenderer.ToBitmapImage(option.Id);
            }

            System.Diagnostics.Debug.WriteLine($"ERROR: Text image renderer is null. Cannot render text for option '{option.Id}'. Returning empty BitmapImage.");
            return new System.Windows.Media.Imaging.BitmapImage();
        }
    }



    public class FocusedOptionRotationConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 4 && values[0] is int focusedIndex && values[1] is int totalCount && values[2] is double width && values[3] is double height)
            {
                double angleStep = 360.0 / totalCount;
                double rotationAngle = focusedIndex * angleStep - 90;

                double centerX = width / 2;
                double centerY = height / 2;

                var rotateTransform = new System.Windows.Media.RotateTransform
                {
                    Angle = rotationAngle,
                    CenterX = centerX,
                    CenterY = centerY
                };

                var transformGroup = new System.Windows.Media.TransformGroup();
                transformGroup.Children.Add(rotateTransform);

                return transformGroup;
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class StringSuffixRemoverConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string inputString)
            {
                string suffixToRemove = parameter as string ?? string.Empty;
                int suffixLength = suffixToRemove.Length;

                if (suffixLength > 0 && inputString.EndsWith(suffixToRemove))
                {
                    string resultString = inputString[..^suffixLength];
                    return resultString;
                }

                return inputString;
            }

            if (value != null)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string inputString)
            {
                if (inputString == string.Empty)
                {
                    return System.Windows.DependencyProperty.UnsetValue;
                }

                string suffixToAppend = parameter as string ?? string.Empty;
                return inputString + suffixToAppend;
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
}
