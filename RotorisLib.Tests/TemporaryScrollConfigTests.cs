namespace RotorisLib.Tests
{
    public class TemporaryScrollConfigTests
    {
        [Fact]
        public void TemporaryScrollConfig_DefaultInitialization_PropertiesAreDefault()
        {
            TemporaryScrollConfig config = new TemporaryScrollConfig();
            Assert.False(config.Enabled);
            Assert.Equal(0, config.IdleTimeoutInSeconds);
            Assert.Null(config.ClickModuleName);
            Assert.Null(config.ClockwiseModuleName);
            Assert.Null(config.CounterclockwiseModuleName);
            Assert.Null(config.TimeoutModuleName);
        }

        [Fact]
        public void TemporaryScrollConfig_PropertyAssignment_WorksCorrectly()
        {
            TemporaryScrollConfig config = new TemporaryScrollConfig
            {
                Enabled = true,
                IdleTimeoutInSeconds = 30,
                ClickModuleName = "ClickModule",
                ClockwiseModuleName = "ClockwiseModule",
                CounterclockwiseModuleName = "CounterclockwiseModule",
                TimeoutModuleName = "TimeoutModule"
            };

            Assert.True(config.Enabled);
            Assert.Equal(30, config.IdleTimeoutInSeconds);
            Assert.Equal("ClickModule", config.ClickModuleName);
            Assert.Equal("ClockwiseModule", config.ClockwiseModuleName);
            Assert.Equal("CounterclockwiseModule", config.CounterclockwiseModuleName);
            Assert.Equal("TimeoutModule", config.TimeoutModuleName);
        }
    }
}