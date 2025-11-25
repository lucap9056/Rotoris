using System.Runtime.InteropServices;
using Rotoris.Logger;

namespace Rotoris.LuaModules
{
    /*
--- A module to simulate keyboard and mouse input using Windows API.
--- @class Rotoris.LuaHID
--- @field key_down fun(self:Rotoris.LuaHID, virtualKey: number) Simulates a key press down event for the specified virtual key code.
--- @field key_up fun(self:Rotoris.LuaHID, virtualKey: number) Simulates a key release event for the specified virtual key code.
--- @field key_press fun(self:Rotoris.LuaHID, virtualKey: number, delayMs?: number) Simulates a key press (down and up) event for the specified virtual key code, with an optional delay in milliseconds between the down and up events. Default delay is 50ms.
--- @field mouse_move fun(self:Rotoris.LuaHID, dx: number, dy: number) Moves the mouse cursor by the specified delta x and delta y values.
--- @field scroll_mouse_wheel fun(self:Rotoris.LuaHID, scrollAmount: number, horizontal?: boolean) Simulates mouse wheel scrolling. If horizontal is true, scrolls horizontally; otherwise, scrolls vertically.
--- @field mouse_click fun(self:Rotoris.LuaHID, flags: number) Simulates a mouse click event with the specified flags (e.g., left click, right click).
     */
    public class LuaHID
    {
        public static readonly string GlobalName = "hid";
        public void key_down(ushort virtualKey)
        {
            User32.INPUT[] inputs = new User32.INPUT[1];
            inputs[0].type = User32.INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = virtualKey;
            inputs[0].U.ki.dwFlags = User32.KEYEVENTF_KEYDOWN;
            uint result = User32.SendInput(1, inputs, Marshal.SizeOf(typeof(User32.INPUT)));
            if (result != 1)
            {
                Log.Warning($"Failed to send key down for virtual key '{virtualKey}'. SendInput returned {result}.");
            }
        }

        public void key_up(ushort virtualKey)
        {
            User32.INPUT[] inputs = new User32.INPUT[1];
            inputs[0].type = User32.INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = virtualKey;
            inputs[0].U.ki.dwFlags = User32.KEYEVENTF_KEYUP;
            uint result = User32.SendInput(1, inputs, Marshal.SizeOf(typeof(User32.INPUT)));
            if (result != 1)
            {
                Log.Warning($"Failed to send key up for virtual key '{virtualKey}'. SendInput returned {result}.");
            }
        }

        public void key_press(ushort virtualKey, int delayMs = 50)
        {
            key_down(virtualKey);
            Thread.Sleep(delayMs);
            key_up(virtualKey);
        }

        public void mouse_move(int dx, int dy)
        {
            User32.INPUT[] inputs = new User32.INPUT[1];
            inputs[0].type = User32.INPUT_MOUSE;
            inputs[0].U.mi.dx = dx;
            inputs[0].U.mi.dy = dy;
            inputs[0].U.mi.dwFlags = User32.MOUSEEVENTF_MOVE;
            uint result = User32.SendInput(1, inputs, Marshal.SizeOf(typeof(User32.INPUT)));
            if (result != 1)
            {
                Log.Warning($"Failed to send mouse move event. SendInput returned {result}.");
            }
        }
        public void scroll_mouse_wheel(int scrollAmount, bool horizontal = false)
        {
            User32.INPUT[] inputs = new User32.INPUT[1];
            inputs[0].type = User32.INPUT_MOUSE;
            inputs[0].U.mi.dwFlags = horizontal ? User32.MOUSEEVENTF_HORIZONTAL_WHEEL : User32.MOUSEEVENTF_STRAIGHT_WHEEL;
            inputs[0].U.mi.mouseData = (uint)scrollAmount;
            inputs[0].U.mi.time = 0;
            inputs[0].U.mi.dx = 0;
            inputs[0].U.mi.dy = 0;

            uint result = User32.SendInput(1, inputs, Marshal.SizeOf(typeof(User32.INPUT)));
            if (result != 1)
            {
                Log.Warning($"Failed to send mouse wheel event. SendInput returned {result}.");
            }
        }

        public void mouse_click(uint flags)
        {
            User32.INPUT[] inputs = new User32.INPUT[1];
            inputs[0].type = User32.INPUT_MOUSE;
            inputs[0].U.mi.dwFlags = flags;
            uint result = User32.SendInput(1, inputs, Marshal.SizeOf(typeof(User32.INPUT)));
            if (result != 1)
            {
                Log.Warning($"Failed to send mouse click event. SendInput returned {result}.");
            }
        }
    }
}
