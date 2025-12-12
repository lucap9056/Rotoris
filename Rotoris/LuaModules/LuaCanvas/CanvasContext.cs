using NLua;
using SkiaSharp;
using System.Drawing;

namespace Rotoris.LuaModules.LuaCanvas
{
    /*
--- @class Rotoris.LuaCanvas.CanvasContext
--- @field paints Rotoris.LuaCanvas.PaintCache
--- @field images Rotoris.LuaCanvas.ImageCache
--- @field fonts Rotoris.LuaCanvas.FontCache
--- @field clear fun(self: Rotoris.LuaCanvas.CanvasContext, hexColor?: string) Clears the canvas with the specified color or transparent if no color is provided.
--- @field set_paint fun(self: Rotoris.LuaCanvas.CanvasContext, name: string) Sets the current paint by name from the PaintCache.
--- @field draw_line fun(self: Rotoris.LuaCanvas.CanvasContext, x1: number, y1: number, x2: number, y2: number) Draws a line between two points.
--- @field draw_oval fun(self: Rotoris.LuaCanvas.CanvasContext, x: number, y: number, rx: number, ry: number) Draws an oval centered at (x, y) with radii rx and ry.
--- @field draw_rect fun(self: Rotoris.LuaCanvas.CanvasContext, x: number, y: number, width: number, height: number, options?: table) Draws a rectangle with optional rounded corners.
--- @field draw_circle fun(self: Rotoris.LuaCanvas.CanvasContext, cx: number, cy: number, radius: number) Draws a circle centered at (cx, cy) with the specified radius.
--- @field draw_text fun(self: Rotoris.LuaCanvas.CanvasContext, text: string, x: number, y: number, fontSize?: number, familyName?: string) Draws text at the specified position.
--- @field measure_text fun(self: Rotoris.LuaCanvas.CanvasContext, text: string, fontSize?: number, familyName?: string): SkiaSharp.SKRect Measures the bounding box of the specified text.
--- @field draw_image fun(self: Rotoris.LuaCanvas.CanvasContext, filePath: string, x: number, y: number, width?: number, height?: number) Draws an image from the specified file path.
--- @field create_path fun(self: Rotoris.LuaCanvas.CanvasContext): Rotoris.LuaCanvas.CanvasPath Creates a new CanvasPath object.
--- @field draw_path fun(self: Rotoris.LuaCanvas.CanvasContext, action: fun(path: Rotoris.LuaCanvas.CanvasPath)) Draws a path defined by the provided action.
--- @field translate fun(self: Rotoris.LuaCanvas.CanvasContext, dx: number, dy: number) Translates the canvas by the specified offsets.
--- @field scale fun(self: Rotoris.LuaCanvas.CanvasContext, sx: number, sy: number) Scales the canvas by the specified factors.
--- @field rotate fun(self: Rotoris.LuaCanvas.CanvasContext, degrees: number) Rotates the canvas by the specified angle in degrees.
--- @field save fun(self: Rotoris.LuaCanvas.CanvasContext) Saves the current canvas state.
--- @field restore fun(self: Rotoris.LuaCanvas.CanvasContext) Restores the canvas to the last saved state.
--- @field delay fun(self: Rotoris.LuaCanvas.CanvasContext, milliseconds: number) Delays execution for the specified number of milliseconds.
--- @field done fun(self: Rotoris.LuaCanvas.CanvasContext): boolean Marks the drawing as done and returns true.
     */
    public class CanvasContext(SKCanvas c) : IDisposable
    {
        /*
--- @class Rotoris.LuaCanvas.CanvasContext.Args
--- @field CanvasWidth number
--- @field CanvasHeight number
--- @field DeltaTime number
         */
        public sealed class Args
        {
            public int CanvasWidth;
            public int CanvasHeight;
            public double DeltaTime;
        }
        public readonly PaintCache paints = new();
        public readonly ImageCache images = new();
        public readonly FontCache fonts = new();
        private readonly SKCanvas canvas = c;
        private bool isDone = false;

        public void clear(string? hexColor = null)
        {
            if (hexColor != null)
            {
                Color color = ColorTranslator.FromHtml(hexColor);
                canvas.Clear(new SKColor(color.R, color.G, color.B, color.A));
            }
            else
            {
                canvas.Clear(SKColors.Transparent);
            }
        }
        public void set_paint(string name)
        {
            paints.set(name);
        }
        public void draw_line(float x1, float y1, float x2, float y2)
        {
            SKPaint paint = paints.get();
            canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        public void draw_oval(float x, float y, float rx, float ry)
        {
            SKPaint paint = paints.get();
            canvas.DrawOval(x, y, rx, ry, paint);
        }

        public struct RectangleRadiusOptions
        {
            public float Radius;
        }

        public struct RectangleRadiiOptions
        {
            public float RadiusX;
            public float RadiusY;
        }

        public void draw_rect(float x, float y, float width, float height, LuaTable? options = null)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    $"Rectangle dimensions must be positive (width={width}, height={height})."
                );
            }

            SKPaint paint = paints.get();

            if (options != null)
            {
                float cornerRadiusX = 0f;
                float cornerRadiusY = 0f;

                if (TryParseRectangleRadiiOptions(options, out var radiiOptions))
                {
                    cornerRadiusX = radiiOptions.RadiusX;
                    cornerRadiusY = radiiOptions.RadiusY;
                }
                else if (TryParseRectangleRadiusOptions(options, out var singleRadiusOptions))
                {
                    cornerRadiusX = singleRadiusOptions.Radius;
                    cornerRadiusY = singleRadiusOptions.Radius;
                }

                if (cornerRadiusX > 0 || cornerRadiusY > 0)
                {
                    canvas.DrawRoundRect(SKRect.Create(x, y, width, height), cornerRadiusX, cornerRadiusY, paint);
                    return;
                }
            }

            canvas.DrawRect(SKRect.Create(x, y, width, height), paint);
        }

        public static bool TryParseRectangleRadiiOptions(LuaTable table, out RectangleRadiiOptions options)
        {
            options = new RectangleRadiiOptions();
            if (table["RadiusX"] != null && table["RadiusY"] != null)
            {
                options.RadiusX = Convert.ToSingle(table["RadiusX"]);
                options.RadiusY = Convert.ToSingle(table["RadiusY"]);
                return true;
            }
            return false;
        }

        public static bool TryParseRectangleRadiusOptions(LuaTable table, out RectangleRadiusOptions options)
        {
            options = new RectangleRadiusOptions();
            if (table["Radius"] != null)
            {
                options.Radius = Convert.ToSingle(table["Radius"]);
                return true;
            }
            return false;
        }

        public void draw_circle(float cx, float cy, float radius)
        {
            if (radius <= 0)
            {
                throw new ArgumentException("Circle radius must be greater than zero.");
            }

            SKPaint paint = paints.get();
            canvas.DrawCircle(cx, cy, radius, paint);
        }

        public void draw_text(string text, float x, float y, int fontSize = 16, string familyName = "Arial")
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (fontSize <= 0)
            {
                throw new ArgumentException("Font size must be greater than zero.");
            }

            SKPaint paint = paints.get();
            var font = fonts.Get(familyName, fontSize);
            using SKTextBlob? textBlob = SKTextBlob.Create(text, font);
            canvas.DrawText(textBlob, x, y, paint);
        }
        public SKRect measure_text(string text, int fontSize = 16, string familyName = "Arial")
        {
            if (string.IsNullOrEmpty(text))
            {
                return SKRect.Empty;
            }
            if (fontSize <= 0)
            {
                throw new ArgumentException("Font size must be greater than zero.");
            }
            var font = fonts.Get(familyName, fontSize);
            font.MeasureText(text, out SKRect bounds);
            return bounds;
        }
        public void draw_image(string filePath, float x, float y, float width = 0, float height = 0)
        {
            SKBitmap? bitmap = images.get(filePath);
            if (bitmap == null)
            {
                return;
            }

            SKRect dest;
            if (width > 0 && height > 0)
            {
                dest = SKRect.Create(x, y, width, height);
            }
            else
            {
                dest = SKRect.Create(x, y, bitmap.Width, bitmap.Height);
            }

            canvas.DrawBitmap(bitmap, dest);
        }
        public CanvasPath create_path()
        {
            return new CanvasPath();
        }
        public void draw_path(LuaFunction action)
        {
            CanvasPath pathEditor = new();
            action.Call(pathEditor);
            SKPath path = pathEditor.get();
            SKPaint paint = paints.get();
            canvas.DrawPath(path, paint);

        }
        public void translate(float dx, float dy)
        {
            canvas.Translate(dx, dy);
        }

        public void scale(float sx, float sy)
        {
            canvas.Scale(sx, sy);
        }

        public void rotate(float degrees)
        {
            canvas.RotateDegrees(degrees);
        }
        public void save()
        {
            canvas.Save();
        }

        public void restore()
        {
            canvas.Restore();
        }
        public void delay(int milliseconds)
        {
            if (milliseconds > 0)
            {
                Thread.Sleep(milliseconds);
            }
        }
        public bool done()
        {
            isDone = true;
            return true;
        }
        public bool IsDone
        {
            get => isDone;
        }
        public void Dispose()
        {
            fonts.Dispose();
            images.Dispose();
            paints.Dispose();
        }
    }
}
