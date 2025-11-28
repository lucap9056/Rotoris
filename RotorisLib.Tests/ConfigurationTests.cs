using RotorisLib;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using Xunit;

namespace RotorisLib.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void Constructor_WithEmptyIniFile_ShouldLoadDefaultValues()
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "");

            try
            {
                var emptyIni = new IniFile(tempFile);

                var config = new Configuration(emptyIni);

                Assert.Equal(Configuration.Default.UiSize, config.UiSize);
                Assert.Equal(Configuration.Default.UiBackground, config.UiBackground);
                Assert.Equal(Configuration.Default.UiForeground, config.UiForeground);
                Assert.Equal(Configuration.Default.UiAccent, config.UiAccent);
                Assert.Equal(Configuration.Default.PrimaryKey, config.PrimaryKey);
                Assert.Equal(Configuration.Default.ClockwiseKey, config.ClockwiseKey);
                Assert.Equal(Configuration.Default.CounterclockwiseKey, config.CounterclockwiseKey);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public void Constructor_WithValidIniFile_ShouldLoadConfiguredValues()
        {
            var iniContent = @"
[APPEARANCE]
Size = 550
Background_Color = FF00FF00
Accent_Color = FFFF0000

[KEY_BINDINGS]
PRIMARY_KEY=+82
";
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, iniContent);

            try
            {
                var iniFile = new IniFile(tempFile);

                var config = new Configuration(iniFile);

                var expectedKey = Hotkey.TryParse("+82", out var key) ? key : default;

                Assert.Equal(550, config.UiSize);
                Assert.Equal(ColorConverter.ConvertFromString("#FF00FF00"), config.UiBackground);
                Assert.Equal(Configuration.Default.UiForeground, config.UiForeground);
                Assert.Equal(ColorConverter.ConvertFromString("#FFFF0000"), config.UiAccent);
                Assert.Equal(expectedKey, config.PrimaryKey);
                Assert.Equal(Configuration.Default.ClockwiseKey, config.ClockwiseKey);
                Assert.Equal(Configuration.Default.CounterclockwiseKey, config.CounterclockwiseKey);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}