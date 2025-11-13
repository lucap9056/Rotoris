namespace RotorisLib
{
    /// <summary>
    /// Provides utility methods and data structures for UI related operations, especially for the radial/ring menu.
    /// </summary>
    public class UserInterface
    {
        /// <summary>
        /// Defines the names of the standard theme brushes used in the UI.
        /// </summary>
        public enum ThemeBrushIdentifier
        {
            /// <summary>The background color brush.</summary>
            Background,
            /// <summary>The foreground color brush, typically for text and icons.</summary>
            Foreground,
            /// <summary>The accent color brush, used for focus or highlight elements.</summary>
            Accent
        }

        /// <summary>
        /// Stores the SolidColorBrush instances for the application's theme settings.
        /// </summary>
        public struct AppThemeBrushes
        {
            /// <summary>Gets or sets the background brush with default opacity.</summary>
            public System.Windows.Media.SolidColorBrush BackgroundBrush { get; set; }

            /// <summary>Gets or sets the foreground brush. Defaults to SystemColors.ControlBrush.</summary>
            public System.Windows.Media.SolidColorBrush ForegroundBrush { get; set; } = System.Windows.SystemColors.ControlBrush;

            /// <summary>Gets or sets the accent brush. Defaults to SystemColors.AccentColorBrush.</summary>
            public System.Windows.Media.SolidColorBrush AccentBrush { get; set; } = System.Windows.SystemColors.AccentColorBrush;

            private const byte DefaultBackgroundOpacity = 204;

            /// <summary>
            /// Initializes a new instance of the AppThemeBrushes class, attempting to fetch the system background color.
            /// </summary>
            public AppThemeBrushes()
            {
                try
                {
                    // Attempt to get Windows UI settings for background color (requires Windows.Foundation.UniversalApiContract reference)
                    var systemSettings = new Windows.UI.ViewManagement.UISettings();
                    var backgroundColor = systemSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);

                    // Convert Windows.UI.Color to System.Windows.Media.Color
                    System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(DefaultBackgroundOpacity, backgroundColor.R, backgroundColor.G, backgroundColor.B);

                    // Create the background brush with default opacity
                    System.Windows.Media.SolidColorBrush backgroundBrush = new(wpfColor);
                    BackgroundBrush = backgroundBrush;
                }
                catch (System.Exception ex)
                {
                    // Fallback to a default color if accessing system settings fails
                    BackgroundBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray); // Fallback brush
                    System.Diagnostics.Debug.WriteLine($"[WARNING] Failed to retrieve system background color. Using default brush. Exception: {ex.Message}");
                }
            }

            /// <summary>Gets the system background color as a System.Windows.Media.Color.</summary>
            public static System.Windows.Media.Color SystemBackgroundColor
            {
                get
                {
                    try
                    {
                        var systemSettings = new Windows.UI.ViewManagement.UISettings();
                        var backgroundColor = systemSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
                        return System.Windows.Media.Color.FromArgb(DefaultBackgroundOpacity, backgroundColor.R, backgroundColor.G, backgroundColor.B);
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ERROR] Could not get SystemBackgroundColor. Returning default white. Exception: {ex.Message}");
                        return System.Windows.Media.Colors.White;
                    }
                }
            }

            /// <summary>Gets the system foreground color as a System.Windows.Media.Color.</summary>
            public static System.Windows.Media.Color SystemForegroundColor
            {
                get => System.Windows.SystemColors.ControlColor;
            }

            /// <summary>Gets the system accent color as a System.Windows.Media.Color.</summary>
            public static System.Windows.Media.Color SystemAccentColor
            {
                get => System.Windows.SystemColors.AccentColor;
            }

            public static System.Windows.Media.SolidColorBrush BrushFromColor(System.Windows.Media.Color color)
            {
                return new System.Windows.Media.SolidColorBrush(color);
            }
        }

        public static System.Windows.Point GetFocusedScreenCenter(System.Windows.Window wpfWindow)
        {
            System.Drawing.Point cursorPosition = System.Windows.Forms.Cursor.Position;
            System.Windows.Forms.Screen? currentScreen = null;

            // Find the screen containing the cursor
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (screen.Bounds.Contains(cursorPosition))
                {
                    currentScreen = screen;
                    break;
                }
            }

            // Fallback to primary screen if the current screen couldn't be determined (shouldn't happen under normal circumstances)
            if (currentScreen == null)
            {
                currentScreen = System.Windows.Forms.Screen.PrimaryScreen ?? System.Windows.Forms.Screen.AllScreens[0];
                System.Diagnostics.Debug.WriteLine("[WARNING] Could not determine current screen based on cursor position. Defaulting to primary screen.");
            }

            // The rest of the original function's logic is missing, so we'll complete it using the primary screen's center as a placeholder, 
            // which the original code structure seemed to be leading toward.
            System.Drawing.Rectangle workingArea = currentScreen.WorkingArea;

            // Calculate screen center in device-independent units (DIP) based on DPI scaling
            System.Windows.PresentationSource? source = System.Windows.PresentationSource.FromVisual(wpfWindow);
            double dpiScaleX = 1.0;
            double dpiScaleY = 1.0;

            if (source != null && source.CompositionTarget != null)
            {
                dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }

            double centerX = (workingArea.Left + workingArea.Width / 2.0) / dpiScaleX;
            double centerY = (workingArea.Top + workingArea.Height / 2.0) / dpiScaleY;

            System.Diagnostics.Debug.WriteLine($"[LOG] Calculated focused screen center at ({centerX:F2}, {centerY:F2}) in DPI-independent units. DPI Scale: {dpiScaleX:F2}x{dpiScaleY:F2}.");

            return new System.Windows.Point(centerX, centerY);
        }
    }

}