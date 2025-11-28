using Xunit;
using RotorisLib;
using System;

namespace RotorisLib.Tests
{
    public class HotkeyTests
    {
        [Fact]
        public void Hotkey_DefaultConstructor_InitializesWithDefaultValues()
        {
            var hotkey = new Hotkey();

            Assert.False(hotkey.IsControlActive);
            Assert.False(hotkey.IsShiftActive);
            Assert.False(hotkey.IsWindowsActive);
            Assert.Equal(0, hotkey.VirtualKeyCode);
        }

        [Fact]
        public void Hotkey_ConstructorWithVkCode_InitializesCorrectly()
        {
            var hotkey = new Hotkey(0x41);

            Assert.False(hotkey.IsControlActive);
            Assert.False(hotkey.IsShiftActive);
            Assert.False(hotkey.IsWindowsActive);
            Assert.Equal(0x41, hotkey.VirtualKeyCode);
        }

        [Fact]
        public void Hotkey_CopyConstructor_CopiesAllProperties()
        {
            var original = new Hotkey(0x42)
            {
                IsControlActive = true,
                IsShiftActive = true
            };
            var copy = new Hotkey(original);

            Assert.Equal(original.VirtualKeyCode, copy.VirtualKeyCode);
            Assert.Equal(original.IsControlActive, copy.IsControlActive);
            Assert.Equal(original.IsShiftActive, copy.IsShiftActive);
            Assert.Equal(original.IsWindowsActive, copy.IsWindowsActive);
        }

        [Fact]
        public void Hotkey_CopyConstructorWithNewVkCode_CopiesModifiersAndSetsNewVkCode()
        {
            var original = new Hotkey(0x41)
            {
                IsControlActive = true,
                IsWindowsActive = true
            };
            var copy = new Hotkey(0x43, original);

            Assert.Equal(0x43, copy.VirtualKeyCode);
            Assert.Equal(original.IsControlActive, copy.IsControlActive);
            Assert.Equal(original.IsShiftActive, copy.IsShiftActive);
            Assert.Equal(original.IsWindowsActive, copy.IsWindowsActive);
        }


        [Theory]
        [InlineData("65", 65, false, false, false)]
        [InlineData("^66", 66, true, false, false)]
        [InlineData("+67", 67, false, true, false)]
        [InlineData("$68", 68, false, false, true)]
        [InlineData("^+69", 69, true, true, false)]
        [InlineData("^$70", 70, true, false, true)]
        [InlineData("+$71", 71, false, true, true)]
        [InlineData("^+ $72", 72, true, true, true)]
        public void Hotkey_TryParse_ValidStrings_ReturnsTrueAndCorrectHotkey(
            string s, int expectedVkCode, bool expectedCtrl, bool expectedShift, bool expectedWin)
        {
            Assert.True(Hotkey.TryParse(s, out var hotkey));
            Assert.NotNull(hotkey);
            Assert.Equal(expectedVkCode, hotkey.VirtualKeyCode);
            Assert.Equal(expectedCtrl, hotkey.IsControlActive);
            Assert.Equal(expectedShift, hotkey.IsShiftActive);
            Assert.Equal(expectedWin, hotkey.IsWindowsActive);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("^invalid")]
        [InlineData("")]
        public void Hotkey_TryParse_InvalidStrings_ReturnsFalseAndNullHotkey(string s)
        {
            Assert.False(Hotkey.TryParse(s, out var hotkey));
            Assert.Equal(hotkey, new Hotkey());
        }


        [Theory]
        [InlineData(65, false, false, false, "65")]
        [InlineData(66, true, false, false, "^66")]
        [InlineData(67, false, true, false, "+67")]
        [InlineData(68, false, false, true, "$68")]
        [InlineData(69, true, true, false, "^+69")]
        [InlineData(70, true, false, true, "^$70")]
        [InlineData(71, false, true, true, "+$71")]
        [InlineData(72, true, true, true, "^+$72")]
        public void Hotkey_ToString_DefaultFormat_ReturnsCorrectString(
            int vkCode, bool ctrl, bool shift, bool win, string expected)
        {
            var hotkey = new Hotkey(vkCode)
            {
                IsControlActive = ctrl,
                IsShiftActive = shift,
                IsWindowsActive = win
            };

            Assert.Equal(expected, hotkey.ToString());
        }

        [Fact]
        public void Hotkey_GetModifiers_ReturnsCorrectFlags()
        {
            var hotkey = new Hotkey(0x01);

            hotkey.IsControlActive = false;
            hotkey.IsShiftActive = false;
            hotkey.IsWindowsActive = false;
            Assert.Equal(Hotkey.HotkeyModifiers.None, hotkey.GetModifiers());

            hotkey.IsControlActive = true;
            Assert.Equal(Hotkey.HotkeyModifiers.Control, hotkey.GetModifiers());

            hotkey.IsShiftActive = true;
            Assert.Equal(Hotkey.HotkeyModifiers.Control | Hotkey.HotkeyModifiers.Shift, hotkey.GetModifiers());

            hotkey.IsControlActive = false;
            hotkey.IsWindowsActive = true;
            Assert.Equal(Hotkey.HotkeyModifiers.Shift | Hotkey.HotkeyModifiers.Windows, hotkey.GetModifiers());
        }

        [Fact]
        public void Hotkey_IsSupersetOf_IdenticalHotkeys_ReturnsTrue()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            var hotkey2 = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            Assert.True(hotkey1.IsSupersetOf(hotkey2));
        }

        [Fact]
        public void Hotkey_IsSupersetOf_CurrentIsSuperset_ReturnsTrue()
        {
            var superset = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            var subset = new Hotkey(0x41) { IsControlActive = true };
            Assert.True(superset.IsSupersetOf(subset));
        }

        [Fact]
        public void Hotkey_IsSupersetOf_CurrentIsNotSuperset_ReturnsFalse()
        {
            var subset = new Hotkey(0x41) { IsControlActive = true };
            var superset = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            Assert.False(subset.IsSupersetOf(superset));
        }

        [Fact]
        public void Hotkey_IsSupersetOf_DifferentVkCode_ReturnsFalse()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true };
            var hotkey2 = new Hotkey(0x42) { IsControlActive = true };
            Assert.False(hotkey1.IsSupersetOf(hotkey2));
        }

        [Fact]
        public void Hotkey_Equals_IdenticalHotkeys_ReturnsTrue()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            var hotkey2 = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            Assert.True(hotkey1.Equals(hotkey2));
            Assert.True(hotkey1.Equals((object)hotkey2));
        }

        [Fact]
        public void Hotkey_Equals_DifferentVkCode_ReturnsFalse()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true };
            var hotkey2 = new Hotkey(0x42) { IsControlActive = true };
            Assert.False(hotkey1.Equals(hotkey2));
            Assert.False(hotkey1.Equals((object)hotkey2));
        }

        [Fact]
        public void Hotkey_Equals_DifferentModifiers_ReturnsFalse()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true };
            var hotkey2 = new Hotkey(0x41) { IsShiftActive = true };
            Assert.False(hotkey1.Equals(hotkey2));
            Assert.False(hotkey1.Equals((object)hotkey2));
        }

        [Fact]
        public void Hotkey_Equals_NullOrDifferentObjectType_ReturnsFalse()
        {
            var hotkey = new Hotkey(0x41);
            Assert.False(hotkey.Equals(new Hotkey()));
            Assert.False(hotkey.Equals(new object()));
        }

        [Fact]
        public void Hotkey_GetHashCode_IdenticalHotkeys_ReturnSameHashCode()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            var hotkey2 = new Hotkey(0x41) { IsControlActive = true, IsShiftActive = true };
            Assert.Equal(hotkey1.GetHashCode(), hotkey2.GetHashCode());
        }

        [Fact]
        public void Hotkey_GetHashCode_DifferentHotkeys_ReturnDifferentHashCode()
        {
            var hotkey1 = new Hotkey(0x41) { IsControlActive = true };
            var hotkey2 = new Hotkey(0x42) { IsControlActive = true };
            Assert.NotEqual(hotkey1.GetHashCode(), hotkey2.GetHashCode());
        }

        [Fact]
        public void Hotkey_ToString_FormatN_RequiresVirtualKeysIntegration()
        {
            var hotkey = new Hotkey(0x41) { IsControlActive = true };
            string expected = "^" + RotorisLib.VirtualKeys.VirtualKeyToString(0x41);
            string actual = hotkey.ToString("n");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Hotkey_ToString_FormatN_UnrecognizedVkCode_ReturnsModifierAndEmptyString()
        {
            var hotkey = new Hotkey(0xFF) { IsShiftActive = true };
            string expected = "+" + RotorisLib.VirtualKeys.VirtualKeyToString(0xFF);
            string actual = hotkey.ToString("n");
            Assert.Equal(expected, actual);
        }
    }
}
