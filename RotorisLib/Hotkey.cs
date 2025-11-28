namespace RotorisLib
{
    /// <summary>
    /// Represents a keyboard hotkey combination, consisting of optional modifier keys and a virtual key code.
    /// </summary>
    public class Hotkey(int vkCode = 0)
    {
        /// <summary>
        /// Defines the bit flags for hotkey modifiers (Ctrl, Shift, Win).
        /// </summary>
        [Flags]
        public enum HotkeyModifiers
        {
            /// <summary>
            /// No modifier key.
            /// </summary>
            None = 0,
            /// <summary>
            /// Control modifier key.
            /// </summary>
            Control = 1,
            /// <summary>
            /// Shift modifier key.
            /// </summary>
            Shift = 2,
            /// <summary>
            /// Windows key modifier.
            /// </summary>
            Windows = 4
        }

        /// <summary>
        /// Defines the character symbols used to represent modifier keys in a string format.
        /// </summary>
        private static class HotkeyModifierSymbols
        {
            /// <summary> The symbol for the Control key. </summary>
            public const char Ctrl = '^';
            /// <summary> The symbol for the Shift key. </summary>
            public const char Shift = '+';
            /// <summary> The symbol for the Windows key. </summary>
            public const char Windows = '$';
        }
        /// <summary>
        /// Attempts to parse a string into a Hotkey object.
        /// The string format should be a combination of modifier symbols ('^', '+', '$') followed by the virtual key code as a string.
        /// </summary>
        /// <param name="s">The string representation of the hotkey.</param>
        /// <param name="hotkey">When this method returns, contains the Hotkey object equivalent to the string, if the conversion succeeded.</param>
        /// <returns><c>true</c> if the string was successfully converted; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string s, out Hotkey hotkey)
        {
            hotkey = new()
            {
                IsControlActive = s.Contains(HotkeyModifierSymbols.Ctrl),
                IsShiftActive = s.Contains(HotkeyModifierSymbols.Shift),
                IsWindowsActive = s.Contains(HotkeyModifierSymbols.Windows),
            };

            string keyString = new([.. s.Where(c =>
                c != HotkeyModifierSymbols.Ctrl &&
                c != HotkeyModifierSymbols.Shift &&
                c != HotkeyModifierSymbols.Windows
                )]);

            if (int.TryParse(keyString, out int keyVkCode))
            {
                hotkey.VirtualKeyCode = keyVkCode;
                return true;
            }

            return false;
        }
        /// <summary> Gets or sets a value indicating whether the Control modifier is active. </summary>
        public bool IsControlActive { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether the Shift modifier is active. </summary>
        public bool IsShiftActive { get; set; } = false;

        /// <summary> Gets or sets a value indicating whether the Windows key modifier is active. </summary>
        public bool IsWindowsActive { get; set; } = false;

        /// <summary> Gets or sets the Virtual Key Code for the hotkey. </summary>
        public int VirtualKeyCode { get; set; } = vkCode;
        public Hotkey(int vkCode, Hotkey other) : this(vkCode)
        {
            IsControlActive = other.IsControlActive;
            IsShiftActive = other.IsShiftActive;
            IsWindowsActive = other.IsWindowsActive;
        }

        public Hotkey(Hotkey other) : this(other.VirtualKeyCode)
        {
            IsControlActive = other.IsControlActive;
            IsShiftActive = other.IsShiftActive;
            IsWindowsActive = other.IsWindowsActive;
        }

        /// <summary>
        /// Returns the string representation of the hotkey, using modifier symbols and the virtual key code as an integer.
        /// </summary>
        /// <returns>A string representing the hotkey.</returns>
        public override string ToString()
        {
            return ToString(string.Empty);
        }
        /// <summary>
        /// Returns the string representation of the hotkey using the specified format.
        /// </summary>
        /// <param name="format">
        /// The format specifier.
        /// "n": Includes the key name (requires a static <c>VirtualKeys.VirtualKeyToString</c> method to be available).
        /// Any other value (or empty string): Includes the key's integer Virtual Key Code.
        /// </param>
        /// <returns>A formatted string representing the hotkey.</returns>
        public string ToString(string format = "")
        {
            string modifierString = string.Empty;

            if (IsControlActive)
            {
                modifierString += HotkeyModifierSymbols.Ctrl;
            }
            if (IsShiftActive)
            {
                modifierString += HotkeyModifierSymbols.Shift;
            }
            if (IsWindowsActive)
            {
                modifierString += HotkeyModifierSymbols.Windows;
            }

            return format.ToLowerInvariant() switch
            {
                "n" => modifierString + VirtualKeys.VirtualKeyToString(VirtualKeyCode),
                _ => modifierString + VirtualKeyCode.ToString()
            };
        }

        /// <summary>
        /// Converts the current modifier properties (Ctrl, Shift, Win) into a single <see cref="HotkeyModifiers"/> flag value.
        /// </summary>
        /// <returns>A <see cref="HotkeyModifiers"/> value representing the active modifiers.</returns>
        public HotkeyModifiers GetModifiers()
        {
            HotkeyModifiers modifierValue = HotkeyModifiers.None;
            if (IsControlActive)
            {
                modifierValue |= HotkeyModifiers.Control;
            }

            if (IsShiftActive)
            {
                modifierValue |= HotkeyModifiers.Shift;
            }

            if (IsWindowsActive)
            {
                modifierValue |= HotkeyModifiers.Windows;
            }

            return modifierValue;
        }
        /// <summary>
        /// Determines if the current hotkey's modifiers are a superset of the target hotkey's modifiers, AND the virtual key codes are identical.
        /// This is often used to check if a pressed key combination (this) matches a registered hotkey (other) where extra modifiers don't negate the match.
        /// (e.g., Ctrl+Shift+A is a superset of Ctrl+A).
        /// </summary>
        /// <param name="other">The hotkey to compare against.</param>
        /// <returns><c>true</c> if the keys are the same and all required modifiers in <paramref name="other"/> are present in the current hotkey; otherwise, <c>false</c>.</returns>
        public bool IsSupersetOf(Hotkey other)
        {
            if (VirtualKeyCode != other.VirtualKeyCode)
            {
                return false;
            }

            HotkeyModifiers thisModifiers = GetModifiers();
            HotkeyModifiers otherModifiers = other.GetModifiers();

            return (thisModifiers & otherModifiers) == otherModifiers;
        }

        /// <summary>
        /// Determines whether the current hotkey is equal to the target hotkey.
        /// Equality requires both the Virtual Key Codes and the exact set of modifiers to be the same.
        /// </summary>
        /// <param name="other">The hotkey to compare with.</param>
        /// <returns><c>true</c> if the hotkeys are identical; otherwise, <c>false</c>.</returns>
        public bool Equals(Hotkey? other)
        {
            if (other is null)
            {
                return false;
            }
            return VirtualKeyCode == other.VirtualKeyCode && GetModifiers() == other.GetModifiers();
        }

        public override bool Equals(object? obj)
        {
            return obj is Hotkey hotkey && Equals(hotkey);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VirtualKeyCode, GetModifiers());
        }
    }
}
