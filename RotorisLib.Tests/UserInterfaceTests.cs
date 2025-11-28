namespace RotorisLib.Tests
{
    public class UserInterfaceTests
    {
        [Fact]
        public void AppThemeBrushes_Properties_SetAndGetCorrectly()
        {
            UserInterface.AppThemeBrushes brushes = new UserInterface.AppThemeBrushes();

            System.Windows.Media.SolidColorBrush customBackground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            System.Windows.Media.SolidColorBrush customForeground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            System.Windows.Media.SolidColorBrush customAccent = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);

            brushes.BackgroundBrush = customBackground;
            brushes.ForegroundBrush = customForeground;
            brushes.AccentBrush = customAccent;

            Assert.Equal(customBackground, brushes.BackgroundBrush);
            Assert.Equal(customForeground, brushes.ForegroundBrush);
            Assert.Equal(customAccent, brushes.AccentBrush);
        }

        [Fact]
        public void AppThemeBrushes_ForegroundAndAccentDefaults_AreSystemColors()
        {
            UserInterface.AppThemeBrushes brushes = new UserInterface.AppThemeBrushes();

            Assert.Equal(System.Windows.SystemColors.ControlBrush, brushes.ForegroundBrush);
            Assert.Equal(System.Windows.SystemColors.AccentColorBrush, brushes.AccentBrush);
        }

        [Fact]
        public void AppThemeBrushes_BrushFromColor_ReturnsCorrectBrush()
        {
            System.Windows.Media.Color testColor = System.Windows.Media.Colors.HotPink;
            System.Windows.Media.SolidColorBrush brush = UserInterface.AppThemeBrushes.BrushFromColor(testColor);

            Assert.NotNull(brush);
            Assert.Equal(testColor, brush.Color);
        }

        [Fact]
        public void AppThemeBrushes_Constructor_UsesUwpDefaultsInTestEnvironment()
        {
            System.IO.StringWriter consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetError(consoleOutput);

            UserInterface.AppThemeBrushes brushes = new UserInterface.AppThemeBrushes();

            Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()));
            Console.SetError(new System.IO.StreamWriter(Console.OpenStandardError()));

            Assert.Equal(System.Windows.Media.Color.FromArgb(204, 0, 0, 0), brushes.BackgroundBrush.Color);
            Assert.DoesNotContain("[WARNING] Failed to retrieve system background color.", consoleOutput.ToString());
        }

        [Fact]
        public void AppThemeBrushes_SystemBackgroundColor_UsesUwpDefaultsInTestEnvironment()
        {
            System.IO.StringWriter consoleOutput = new System.IO.StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetError(consoleOutput);

            System.Windows.Media.Color backgroundColor = UserInterface.AppThemeBrushes.SystemBackgroundColor;

            Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()));
            Console.SetError(new System.IO.StreamWriter(Console.OpenStandardError()));

            Assert.Equal(System.Windows.Media.Color.FromArgb(204, 0, 0, 0), backgroundColor);
            Assert.DoesNotContain("[ERROR] Could not get SystemBackgroundColor.", consoleOutput.ToString());
        }
    }
}