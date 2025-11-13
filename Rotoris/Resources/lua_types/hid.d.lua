--- A module to simulate keyboard and mouse input using Windows API.
--- @class Rotoris.LuaHID
--- @field key_down fun(self:Rotoris.LuaHID, virtualKey: number) Simulates a key press down event for the specified virtual key code.
--- @field key_up fun(self:Rotoris.LuaHID, virtualKey: number) Simulates a key release event for the specified virtual key code.
--- @field key_press fun(self:Rotoris.LuaHID, virtualKey: number, delayMs?: number) Simulates a key press (down and up) event for the specified virtual key code, with an optional delay in milliseconds between the down and up events. Default delay is 50ms.
--- @field mouse_move fun(self:Rotoris.LuaHID, dx: number, dy: number) Moves the mouse cursor by the specified delta x and delta y values.
--- @field scroll_mouse_wheel fun(self:Rotoris.LuaHID, scrollAmount: number, horizontal?: boolean) Simulates mouse wheel scrolling. If horizontal is true, scrolls horizontally; otherwise, scrolls vertically.
--- @field mouse_click fun(self:Rotoris.LuaHID, flags: number) Simulates a mouse click event with the specified flags (e.g., left click, right click).

--- @class Rotoris.LuaHID
local hid = {}

hid:key_down(0x41)                 -- Simulate 'A' key down
hid:key_up(0x41)                   -- Simulate 'A' key up
hid:key_press(0x42, 100)           -- Simulate 'B' key press with 100ms delay
hid:mouse_move(100, 50)            -- Move mouse cursor by (100, 50)
hid:scroll_mouse_wheel(120)        -- Scroll mouse wheel up
hid:scroll_mouse_wheel(-120, true) -- Scroll mouse wheel left

hid:mouse_click(0x0002)            -- Simulate right mouse button click
hid:mouse_click(0x0004)            -- Simulate right mouse button release
hid:mouse_click(0x0001)            -- Simulate left mouse button click
hid:mouse_click(0x0008)            -- Simulate left mouse button release
