--- @class Rotoris.LuaCanvas
--- @field draw fun(self: Rotoris.LuaCanvas, table: Rotoris.LuaCanvas.DrawContext) Starts a drawing session using the provided drawing context.
--- @field set_size fun(self: Rotoris.LuaCanvas, width?: number, height?: number) Sets the size of the canvas. If no dimensions are provided, defaults to the initial size.
--- @field clear fun(self: Rotoris.LuaCanvas) Clears the canvas to transparent.

--- @class Rotoris.LuaCanvas.DrawContext
--- @field OnInit fun(ctx: Rotoris.LuaCanvas.CanvasContext, args: Rotoris.LuaCanvas.CanvasContext.Args): any Initializes the drawing context. Returns an optional state object.
--- @field OnUpdate? fun(ctx: Rotoris.LuaCanvas.CanvasContext, args: Rotoris.LuaCanvas.CanvasContext.Args, state: any) Updates the drawing context. Called repeatedly until done.
