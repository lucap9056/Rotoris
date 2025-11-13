namespace RotorisLib
{
    /// <summary>
    /// Provides utility methods and enumerations for working with virtual key codes,
    /// covering keyboard, mouse buttons, and mouse wheel actions.
    /// </summary>
    public class VirtualKeys
    {
        /// <summary>
        /// Represents the virtual key codes for standard mouse buttons.
        /// </summary>
        public enum Mouse
        {
            /// <summary>No mouse button.</summary>
            None,
            /// <summary>Left mouse button.</summary>
            Left,
            /// <summary>Right mouse button.</summary>
            Right,
            /// <summary>Cancel button (often used for Break).</summary>
            Cancel,
            /// <summary>Middle mouse button.</summary>
            Middle,
            /// <summary>First extended mouse button.</summary>
            XButton1,
            /// <summary>Second extended mouse button.</summary>
            XButton2
        }

        /// <summary>
        /// Represents custom virtual key codes used to distinguish mouse wheel actions.
        /// These codes are negative to avoid conflict with standard Windows virtual key codes (0x01-0xFF).
        /// </summary>
        public enum Wheel
        {
            /// <summary>No wheel action.</summary>
            None = 0,
            /// <summary>Mouse wheel scroll up.</summary>
            WheelUp = -1,
            /// <summary>Mouse wheel scroll down.</summary>
            WheelDown = -2,
            /// <summary>Mouse wheel scroll left (tilt).</summary>
            WheelLeft = -3,
            /// <summary>Mouse wheel scroll right (tilt).</summary>
            WheelRight = -4,
        }
        public enum Keyboard
        {
            /// <summary>Left Control key.</summary>
            LeftControl = 0xA2,
            /// <summary>Left Shift key.</summary>
            LeftShift = 0xA0,
            /// <summary>Left Windows key (Start button).</summary>
            LeftWindows = 0x5B,

            /// <summary>Right Control key.</summary>
            RightControl = 0xA3,
            /// <summary>Right Shift key.</summary>
            RightShift = 0xA1,
            /// <summary>Right Windows key (Start button).</summary>
            RightWindows = 0x5C,

            /// <summary>OEM specific: Attention key.</summary>
            OemAttn = 0xF0,
            /// <summary>OEM specific: Finish key.</summary>
            OemFinish = 0xF1
        }
        /// <summary>
        /// Attempts to convert a Windows virtual key code to a WPF <see cref="System.Windows.Input.Key"/>.
        /// </summary>
        /// <param name="virtualKey">The Windows virtual key code.</param>
        /// <param name="key">When this method returns, contains the equivalent WPF key, if the conversion succeeded; otherwise, <see cref="System.Windows.Input.Key.None"/>.</param>
        /// <returns><see langword="true"/> if the conversion was successful; otherwise, <see langword="false"/>.</returns>
        public static bool KeyFromVirtualKey(int virtualKey, out System.Windows.Input.Key key)
        {
            key = System.Windows.Input.KeyInterop.KeyFromVirtualKey(virtualKey);
            if (key == System.Windows.Input.Key.None)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to convert a virtual key code (representing a mouse button) to a WPF <see cref="System.Windows.Input.MouseButton"/>.
        /// </summary>
        /// <param name="virtualKey">The virtual key code, expected to be one of the <see cref="Mouse"/> values.</param>
        /// <param name="button">When this method returns, contains the equivalent WPF mouse button, if the conversion succeeded; otherwise, <see cref="System.Windows.Input.MouseButton.Left"/> (as a default).</param>
        /// <returns><see langword="true"/> if the conversion was successful and the virtual key corresponds to a mouse button; otherwise, <see langword="false"/>.</returns>
        public static bool MouseButtonFromVirtualKey(int virtualKey, out System.Windows.Input.MouseButton button)
        {
            System.Windows.Input.MouseButton? b = virtualKey switch
            {
                (int)Mouse.Left => System.Windows.Input.MouseButton.Left,
                (int)Mouse.Right => System.Windows.Input.MouseButton.Right,
                (int)Mouse.Middle => System.Windows.Input.MouseButton.Middle,
                (int)Mouse.XButton1 => System.Windows.Input.MouseButton.XButton1,
                (int)Mouse.XButton2 => System.Windows.Input.MouseButton.XButton2,
                _ => null,
            };

            button = b ?? System.Windows.Input.MouseButton.Left;

            if (b == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to convert a custom virtual key code (representing a mouse wheel action) to a <see cref="Wheel"/>.
        /// </summary>
        /// <param name="virtualKey">The virtual key code, expected to be one of the <see cref="Wheel"/> values (negative integers).</param>
        /// <param name="wheel">When this method returns, contains the equivalent mouse wheel code, if the conversion succeeded; otherwise, <see cref="Wheel.None"/>.</param>
        /// <returns><see langword="true"/> if the conversion was successful and the virtual key corresponds to a mouse wheel action; otherwise, <see langword="false"/>.</returns>
        public static bool WheelFromVirtualKey(int virtualKey, out Wheel wheel)
        {
            if (virtualKey >= 0)
            {
                wheel = Wheel.None;
                return false;
            }
            wheel = virtualKey switch
            {
                (int)Wheel.WheelUp => Wheel.WheelUp,
                (int)Wheel.WheelDown => Wheel.WheelDown,
                (int)Wheel.WheelLeft => Wheel.WheelLeft,
                (int)Wheel.WheelRight => Wheel.WheelRight,
                _ => Wheel.None,
            };
            return wheel != Wheel.None;
        }

        /// <summary>
        /// Converts a virtual key code (keyboard, mouse button, or wheel) into its string representation.
        /// Prioritizes Mouse Wheel, then Mouse Button, then Keyboard Key.
        /// </summary>
        /// <param name="virtualKey">The virtual key code.</param>
        /// <returns>The string representation of the key code, or an empty string if the code is not recognized.</returns>
        public static string VirtualKeyToString(int virtualKey)
        {
            if (WheelFromVirtualKey(virtualKey, out var wheel))
            {
                return wheel.ToString();
            }
            if (MouseButtonFromVirtualKey(virtualKey, out var button))
            {
                return button.ToString();
            }

            if (KeyFromVirtualKey(virtualKey, out var key))
            {
                return key.ToString();
            }
            return "";
        }

        /// <summary>
        /// Converts a WPF <see cref="System.Windows.Input.Key"/> to its corresponding Windows virtual key code.
        /// </summary>
        /// <param name="key">The WPF key.</param>
        /// <returns>The integer Windows virtual key code.</returns>
        public static int ToVirtualKey(System.Windows.Input.Key key)
        {
            return System.Windows.Input.KeyInterop.VirtualKeyFromKey(key);
        }

        /// <summary>
        /// Converts a WPF <see cref="System.Windows.Input.MouseButton"/> to its corresponding custom virtual key code (<see cref="MouseButtonCode"/>).
        /// </summary>
        /// <param name="button">The WPF mouse button.</param>
        /// <returns>The integer custom virtual key code.</returns>
        public static int ToVirtualKey(System.Windows.Input.MouseButton button)
        {
            Mouse key = button switch
            {
                System.Windows.Input.MouseButton.Left => Mouse.Left,
                System.Windows.Input.MouseButton.Right => Mouse.Right,
                System.Windows.Input.MouseButton.Middle => Mouse.Middle,
                System.Windows.Input.MouseButton.XButton1 => Mouse.XButton1,
                System.Windows.Input.MouseButton.XButton2 => Mouse.XButton2,
                _ => Mouse.None,
            };

            return (int)key;
        }

        /// <summary>
        /// Converts a <see cref="Wheel"/> to its corresponding custom virtual key code.
        /// </summary>
        /// <param name="wheelCode">The mouse wheel action code.</param>
        /// <returns>The integer custom virtual key code (a negative integer).</returns>
        public static int ToVirtualKey(Wheel wheel)
        {
            return (int)wheel;
        }
    }
}
