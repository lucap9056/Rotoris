namespace RotorisLib.Tests
{
    public class VirtualKeysTests
    {
        [Theory]
        [InlineData(0x41, System.Windows.Input.Key.A, true)]
        [InlineData(0x11, System.Windows.Input.Key.LeftCtrl, true)]
        [InlineData(0x01, System.Windows.Input.Key.None, false)]
        [InlineData(0x00, System.Windows.Input.Key.None, false)]
        public void KeyFromVirtualKey_ConvertsCorrectly(int virtualKey, System.Windows.Input.Key expectedKey, bool expectedResult)
        {
            bool result = VirtualKeys.KeyFromVirtualKey(virtualKey, out System.Windows.Input.Key actualKey);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedKey, actualKey);
        }

        [Theory]
        [InlineData((int)VirtualKeys.Mouse.Left, System.Windows.Input.MouseButton.Left, true)]
        [InlineData((int)VirtualKeys.Mouse.Right, System.Windows.Input.MouseButton.Right, true)]
        [InlineData((int)VirtualKeys.Mouse.Middle, System.Windows.Input.MouseButton.Middle, true)]
        [InlineData((int)VirtualKeys.Mouse.XButton1, System.Windows.Input.MouseButton.XButton1, true)]
        [InlineData((int)VirtualKeys.Mouse.XButton2, System.Windows.Input.MouseButton.XButton2, true)]
        [InlineData(0x41, System.Windows.Input.MouseButton.Left, false)]
        [InlineData(0x00, System.Windows.Input.MouseButton.Left, false)]
        public void MouseButtonFromVirtualKey_ConvertsCorrectly(int virtualKey, System.Windows.Input.MouseButton expectedButton, bool expectedResult)
        {
            bool result = VirtualKeys.MouseButtonFromVirtualKey(virtualKey, out System.Windows.Input.MouseButton actualButton);
            Assert.Equal(expectedResult, result);
            if (expectedResult || expectedButton == System.Windows.Input.MouseButton.Left)
            {
                Assert.Equal(expectedButton, actualButton);
            }
        }

        [Theory]
        [InlineData((int)VirtualKeys.Wheel.WheelUp, VirtualKeys.Wheel.WheelUp, true)]
        [InlineData((int)VirtualKeys.Wheel.WheelDown, VirtualKeys.Wheel.WheelDown, true)]
        [InlineData((int)VirtualKeys.Wheel.WheelLeft, VirtualKeys.Wheel.WheelLeft, true)]
        [InlineData((int)VirtualKeys.Wheel.WheelRight, VirtualKeys.Wheel.WheelRight, true)]
        [InlineData(0x41, VirtualKeys.Wheel.None, false)]
        [InlineData(-99, VirtualKeys.Wheel.None, false)]
        [InlineData(0, VirtualKeys.Wheel.None, false)]
        public void WheelFromVirtualKey_ConvertsCorrectly(int virtualKey, VirtualKeys.Wheel expectedWheel, bool expectedResult)
        {
            bool result = VirtualKeys.WheelFromVirtualKey(virtualKey, out VirtualKeys.Wheel actualWheel);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedWheel, actualWheel);
        }

        [Theory]
        [InlineData((int)VirtualKeys.Wheel.WheelUp, "WheelUp")]
        [InlineData((int)VirtualKeys.Mouse.Left, "Left")]
        [InlineData(0x41, "A")]
        [InlineData(0x11, "LeftCtrl")]
        [InlineData(0x00, "")]
        [InlineData(-99, "")]
        public void VirtualKeyToString_ReturnsCorrectString(int virtualKey, string expectedString)
        {
            string actualString = VirtualKeys.VirtualKeyToString(virtualKey);
            Assert.Equal(expectedString, actualString);
        }

        [Theory]
        [InlineData(System.Windows.Input.Key.A, 0x41)]
        [InlineData(System.Windows.Input.Key.LeftCtrl, 0xA2)]
        [InlineData(System.Windows.Input.Key.Sleep, 0x5F)]
        public void ToVirtualKey_Key_ConvertsCorrectly(System.Windows.Input.Key key, int expectedVkCode)
        {
            int actualVkCode = VirtualKeys.ToVirtualKey(key);
            Assert.Equal(expectedVkCode, actualVkCode);
        }

        [Theory]
        [InlineData(System.Windows.Input.MouseButton.Left, (int)VirtualKeys.Mouse.Left)]
        [InlineData(System.Windows.Input.MouseButton.Right, (int)VirtualKeys.Mouse.Right)]
        [InlineData(System.Windows.Input.MouseButton.Middle, (int)VirtualKeys.Mouse.Middle)]
        [InlineData(System.Windows.Input.MouseButton.XButton1, (int)VirtualKeys.Mouse.XButton1)]
        [InlineData(System.Windows.Input.MouseButton.XButton2, (int)VirtualKeys.Mouse.XButton2)]
        public void ToVirtualKey_MouseButton_ConvertsCorrectly(System.Windows.Input.MouseButton button, int expectedVkCode)
        {
            int actualVkCode = VirtualKeys.ToVirtualKey(button);
            Assert.Equal(expectedVkCode, actualVkCode);
        }

        [Theory]
        [InlineData(VirtualKeys.Wheel.WheelUp, (int)VirtualKeys.Wheel.WheelUp)]
        [InlineData(VirtualKeys.Wheel.WheelDown, (int)VirtualKeys.Wheel.WheelDown)]
        [InlineData(VirtualKeys.Wheel.WheelLeft, (int)VirtualKeys.Wheel.WheelLeft)]
        [InlineData(VirtualKeys.Wheel.WheelRight, (int)VirtualKeys.Wheel.WheelRight)]
        public void ToVirtualKey_Wheel_ConvertsCorrectly(VirtualKeys.Wheel wheel, int expectedVkCode)
        {
            int actualVkCode = VirtualKeys.ToVirtualKey(wheel);
            Assert.Equal(expectedVkCode, actualVkCode);
        }
    }
}