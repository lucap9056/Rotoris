using Xunit;
using RotorisLib;
using System.Linq;

namespace RotorisLib.Tests
{
    public class ActionModuleTests
    {
        [Fact]
        public void ActionModule_DefaultInitialization_PropertiesAreDefault()
        {
            ActionModule module = new ActionModule();
            Assert.False(module.CallNext);
            Assert.Null(module.Script);
        }

        [Fact]
        public void ActionModule_PropertyAssignment_WorksCorrectly()
        {
            ActionModule module = new ActionModule
            {
                CallNext = true,
                Script = "test script content"
            };

            Assert.True(module.CallNext);
            Assert.Equal("test script content", module.Script);
        }

        [Theory]
        [InlineData("--!call-next some script", true, "--!call-next some script")]
        [InlineData("  --!call-next another script", true, "  --!call-next another script")]
        [InlineData("some script without call-next", false, "some script without call-next")]
        [InlineData(" --!call-next-not-quite-right", true, " --!call-next-not-quite-right")]
        [InlineData("just a script", false, "just a script")]
        [InlineData("", false, "")]

        public void ActionModule_Constructor_ParsesCallNextAndScriptCorrectly(string inputScript, bool expectedCallNext, string expectedScript)
        {
            ActionModule module = new ActionModule(inputScript);

            Assert.Equal(expectedCallNext, module.CallNext);
            Assert.Equal(expectedScript, module.Script);
        }
    }
}