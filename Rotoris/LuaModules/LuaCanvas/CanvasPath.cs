using SkiaSharp;

namespace Rotoris.LuaModules.LuaCanvas
{
    /*
--- @class Rotoris.LuaCanvas.CanvasPath
--- @field reset fun(self: Rotoris.LuaCanvas.CanvasPath) Resets the path, clearing all segments and points.
--- @field move_to fun(self: Rotoris.LuaCanvas.CanvasPath, x: number, y: number) Moves the current point to (x, y), starting a new contour.
--- @field line_to fun(self: Rotoris.LuaCanvas.CanvasPath, x: number, y: number) Draws a line segment from the current point to (x, y).
--- @field quad_to fun(self: Rotoris.LuaCanvas.CanvasPath, x1: number, y1: number, x2: number, y2: number) Draws a Quadratic Bezier curve to (x2, y2), using (x1, y1) as the control point.
--- @field cubic_to fun(self: Rotoris.LuaCanvas.CanvasPath, x1: number, y1: number, x2: number, y2: number, x3: number, y3: number) Draws a Cubic Bezier curve to (x3, y3), using (x1, y1) and (x2, y2) as the control points.
--- @field close fun(self: Rotoris.LuaCanvas.CanvasPath) Closes the current contour.
--- @field add_rect fun(self: Rotoris.LuaCanvas.CanvasPath, x: number, y: number, width: number, height: number, direction?: SkiaSharp.SKPathDirection) Adds a rectangle to the path.
--- @field add_circle fun(self: Rotoris.LuaCanvas.CanvasPath, cx: number, cy: number, radius: number, direction?: SkiaSharp.SKPathDirection) Adds a circle to the path.
--- @field add_arc fun(self: Rotoris.LuaCanvas.CanvasPath, x: number, y: number, width: number, height: number, startAngle: number, sweepAngle: number, forceMoveTo?: boolean) Adds an arc segment to the path as a new contour.
--- @field add_oval fun(self: Rotoris.LuaCanvas.CanvasPath, x: number, y: number, width: number, height: number, direction?: SkiaSharp.SKPathDirection) Adds an oval (ellipse) to the path.
--- @field add_path fun(self: Rotoris.LuaCanvas.CanvasPath, anotherPath: Rotoris.LuaCanvas.CanvasPath|SkiaSharp.SKPath, x?: number, y?: number) Adds all contours from another path to the current path, with optional offset.
--- @field add_path_reverse fun(self: Rotoris.LuaCanvas.CanvasPath, anotherPath: Rotoris.LuaCanvas.CanvasPath|SkiaSharp.SKPath) Adds all contours from another path in reverse to the current path.
--- @field add_poly fun(self: Rotoris.LuaCanvas.CanvasPath, points: SkiaSharp.SKPoint[], close?: boolean) Adds a polygon to the path.
--- @field add_round_rect fun(self: Rotoris.LuaCanvas.CanvasPath, x: number, y: number, width: number, height: number, rx: number, ry: number, direction?: SkiaSharp.SKPathDirection) Adds a rounded rectangle to the path.
--- @field arc_to fun(self: Rotoris.LuaCanvas.CanvasPath, p1X: number, p1Y: number, p2X: number, p2Y: number, radius: number) Adds a circular arc segment to (x2, y2), using (x1, y1) as the tangent control point.
--- @field set_fill_type fun(self: Rotoris.LuaCanvas.CanvasPath, fillType: SkiaSharp.SKPathFillType) Sets the path's fill rule.
--- @field get_fill_type fun(self: Rotoris.LuaCanvas.CanvasPath): SkiaSharp.SKPathFillType Gets the path's fill rule.
--- @field get fun(self: Rotoris.LuaCanvas.CanvasPath): SkiaSharp.SKPath Gets the internal SKPath object.
     */
    public class CanvasPath
    {
        private readonly SKPath path = new();

        /// <summary>
        /// Resets the path, clearing all segments and points.
        /// </summary>
        public void reset()
        {
            path.Reset();
        }

        /// <summary>
        /// Moves the current point to the specified coordinates (x, y), starting a new contour.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void move_to(float x, float y)
        {
            path.MoveTo(x, y);
        }

        /// <summary>
        /// Draws a line segment from the current point to the specified coordinates (x, y).
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public void line_to(float x, float y)
        {
            path.LineTo(x, y);
        }

        /// <summary>
        /// Draws a Quadratic Bezier curve to (x2, y2), using (x1, y1) as the control point.
        /// </summary>
        /// <param name="x1">The control point X coordinate.</param>
        /// <param name="y1">The control point Y coordinate.</param>
        /// <param name="x2">The end point X coordinate.</param>
        /// <param name="y2">The end point Y coordinate.</param>
        public void quad_to(float x1, float y1, float x2, float y2)
        {
            path.QuadTo(x1, y1, x2, y2);
        }

        /// <summary>
        /// Draws a Cubic Bezier curve to (x3, y3), using (x1, y1) and (x2, y2) as the control points.
        /// </summary>
        /// <param name="x1">The first control point X coordinate.</param>
        /// <param name="y1">The first control point Y coordinate.</param>
        /// <param name="x2">The second control point X coordinate.</param>
        /// <param name="y2">The second control point Y coordinate.</param>
        /// <param name="x3">The end point X coordinate.</param>
        /// <param name="y3">The end point Y coordinate.</param>
        public void cubic_to(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            path.CubicTo(x1, y1, x2, y2, x3, y3);
        }

        /// <summary>
        /// Closes the current contour by drawing a line segment from the current point back to the contour's starting point.
        /// </summary>
        public void close()
        {
            path.Close();
        }

        /// <summary>
        /// Adds a rectangle to the path.
        /// </summary>
        /// <param name="x">The X coordinate of the top-left corner (Left).</param>
        /// <param name="y">The Y coordinate of the top-left corner (Top).</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="direction">The path direction (Clockwise/Counter-clockwise).</param>
        public void add_rect(float x, float y, float width, float height, SKPathDirection direction = SKPathDirection.Clockwise)
        {
            path.AddRect(SKRect.Create(x, y, width, height), direction);
        }

        /// <summary>
        /// Adds a circle to the path.
        /// </summary>
        /// <param name="cx">The X coordinate of the circle's center.</param>
        /// <param name="cy">The Y coordinate of the circle's center.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="direction">The path direction (Clockwise/Counter-clockwise).</param>
        public void add_circle(float cx, float cy, float radius, SKPathDirection direction = SKPathDirection.Clockwise)
        {
            path.AddCircle(cx, cy, radius, direction);
        }

        // --- New Path Addition and Drawing Methods ---

        /// <summary>
        /// Adds an arc segment to the path as a new contour.
        /// </summary>
        /// <param name="x">The X coordinate of the top-left corner of the oval bounds (Left).</param>
        /// <param name="y">The Y coordinate of the top-left corner of the oval bounds (Top).</param>
        /// <param name="width">The width of the oval bounds.</param>
        /// <param name="height">The height of the oval bounds.</param>
        /// <param name="startAngle">The starting angle (in degrees).</param>
        /// <param name="sweepAngle">The sweep angle (in degrees).</param>
        /// <param name="forceMoveTo">Whether to force starting a new contour with move_to (Currently ignored as SKPath.AddArc is used directly).</param>
        public void add_arc(float x, float y, float width, float height, float startAngle, float sweepAngle, bool forceMoveTo = true)
        {
            path.AddArc(SKRect.Create(x, y, width, height), startAngle, sweepAngle);
            if (forceMoveTo)
            {
                // AddArc connects to the previous contour if not closed. 
                // This implementation uses the simplest SKPath.AddArc signature.
            }
        }

        /// <summary>
        /// Adds an oval (ellipse) to the path.
        /// </summary>
        /// <param name="x">The X coordinate of the top-left corner of the oval bounds (Left).</param>
        /// <param name="y">The Y coordinate of the top-left corner of the oval bounds (Top).</param>
        /// <param name="width">The width of the oval bounds.</param>
        /// <param name="height">The height of the oval bounds.</param>
        /// <param name="direction">The path direction (Clockwise/Counter-clockwise).</param>
        public void add_oval(float x, float y, float width, float height, SKPathDirection direction = SKPathDirection.Clockwise)
        {
            path.AddOval(SKRect.Create(x, y, width, height), direction);
        }

        /// <summary>
        /// Adds all contours from another SKPath to the current path.
        /// </summary>
        /// <param name="anotherPath">The CanvasPath object to add.</param>
        /// <param name="x">Optional: X offset.</param>
        /// <param name="y">Optional: Y offset.</param>
        public void add_path(CanvasPath anotherPath, float x = 0, float y = 0)
        {
            add_path(anotherPath.get(), x, y);
        }
        /// <summary>
        /// Adds all contours from another SKPath to the current path.
        /// </summary>
        /// <param name="anotherPath">The SKPath object to add.</param>
        /// <param name="x">Optional: X offset.</param>
        /// <param name="y">Optional: Y offset.</param>
        public void add_path(SKPath anotherPath, float x = 0, float y = 0)
        {
            if (x != 0 || y != 0)
            {
                // Use the SKPath.AddPath overload with translation
                path.AddPath(anotherPath, x, y);
            }
            else
            {
                path.AddPath(anotherPath);
            }
        }
        /// <summary>
        /// Adds all contours from another SKPath in reverse to the current path.
        /// </summary>
        /// <param name="anotherPath">The CanvasPath object to add in reverse.</param>
        public void add_path_reverse(CanvasPath anotherPath)
        {
            add_path_reverse(anotherPath.get());
        }
        /// <summary>
        /// Adds all contours from another SKPath in reverse to the current path.
        /// </summary>
        /// <param name="anotherPath">The SKPath object to add in reverse.</param>
        public void add_path_reverse(SKPath anotherPath)
        {
            path.AddPathReverse(anotherPath);
        }

        /// <summary>
        /// Adds a polygon to the path, optionally closing it (Close() is called by default).
        /// </summary>
        /// <param name="points">An array of vertices for the polygon.</param>
        /// <param name="close">Whether to close the polygon (default is true).</param>
        public void add_poly(SKPoint[] points, bool close = true)
        {
            path.AddPoly(points, close);
        }

        /// <summary>
        /// Adds a rounded rectangle to the path.
        /// </summary>
        /// <param name="x">The X coordinate of the top-left corner (Left).</param>
        /// <param name="y">The Y coordinate of the top-left corner (Top).</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="rx">The corner radius in the X direction.</param>
        /// <param name="ry">The corner radius in the Y direction.</param>
        /// <param name="direction">The path direction (Clockwise/Counter-clockwise).</param>
        public void add_round_rect(float x, float y, float width, float height, float rx, float ry, SKPathDirection direction = SKPathDirection.Clockwise)
        {
            path.AddRoundRect(SKRect.Create(x, y, width, height), rx, ry, direction);
        }

        /// <summary>
        /// Adds a circular arc segment to (x2, y2), using (x1, y1) as the tangent control point. This method is often used for rounding corners.
        /// </summary>
        /// <param name="p1X">The X coordinate of the arc's tangent control point.</param>
        /// <param name="p1Y">The Y coordinate of the arc's tangent control point.</param>
        /// <param name="p2X">The X coordinate of the arc's end point.</param>
        /// <param name="p2Y">The Y coordinate of the arc's end point.</param>
        /// <param name="radius">The radius of the arc.</param>
        public void arc_to(float p1X, float p1Y, float p2X, float p2Y, float radius)
        {
            path.ArcTo(p1X, p1Y, p2X, p2Y, radius);
        }

        // --- Path Fill Rules ---

        /// <summary>
        /// Sets the path's fill rule (Fill Type).
        /// </summary>
        /// <param name="fillType">The fill rule, such as Winding (default) or EvenOdd.</param>
        public void set_fill_type(SKPathFillType fillType)
        {
            path.FillType = fillType;
        }

        /// <summary>
        /// Gets the path's fill rule.
        /// </summary>
        /// <returns>The current SKPathFillType.</returns>
        public SKPathFillType get_fill_type()
        {
            return path.FillType;
        }

        /// <summary>
        /// Gets the internal SKPath object.
        /// </summary>
        /// <returns>The SKPath object.</returns>
        public SKPath get()
        {
            return path;
        }
    }
}