using NLua;
using Rotoris.Logger;
using SkiaSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Rotoris.LuaModules.LuaCanvas
{
    /*
--- @class Rotoris.LuaCanvas
--- @field draw fun(self: Rotoris.LuaCanvas, table: Rotoris.LuaCanvas.DrawContext) Starts a drawing session using the provided drawing context.
--- @field set_size fun(self: Rotoris.LuaCanvas, width?: number, height?: number) Sets the size of the canvas. If no dimensions are provided, defaults to the initial size.
--- @field clear fun(self: Rotoris.LuaCanvas) Clears the canvas to transparent.
     */
    public class LuaCanvas : IDisposable
    {
        public static readonly string GlobalName = "canvas";
        public static readonly string Module = @"
import 'SkiaSharp'

local function clamp(value, min_value, max_value)
    return math.max(min_value, math.min(value, max_value))
end

function color_from_rgba(r, g, b, a)
    r = r or 255
    g = g or 255
    b = b or 255
    a = a or 255

    a = clamp(a, 0, 255)
    r = clamp(r, 0, 255)
    g = clamp(g, 0, 255)
    b = clamp(b, 0, 255)

    return string.format(""#%02X%02X%02X%02X"", a, r, g, b)
end

return {
    color_from_rgba = color_from_rgba,
    PaintStyle = luanet.SkiaSharp.SKPaintStyle,
    TextAlign = luanet.SkiaSharp.SKTextAlign,
    StrokeCap = luanet.SkiaSharp.SKStrokeCap,
    StrokeJoin = luanet.SkiaSharp.SKStrokeJoin,
    BlendMode = luanet.SkiaSharp.SKBlendMode,
    PathEffect = {
        CreateDash = luanet.SkiaSharp.SKPathEffect.CreateDash,
        CreateDiscrete = luanet.SkiaSharp.SKPathEffect.CreateDiscrete,
        CreateTrim = luanet.SkiaSharp.SKPathEffect.CreateTrim,
    },
    Shader = {
        CreateLinearGradient = luanet.SkiaSharp.SKShader.CreateLinearGradient,
        CreateRadialGradient = luanet.SkiaSharp.SKShader.CreateRadialGradient,
        CreateSweepGradient = luanet.SkiaSharp.SKShader.CreateSweepGradient,
        CreateBitmap = luanet.SkiaSharp.SKShader.CreateBitmap,
    },
    PathDirection = luanet.SkiaSharp.SKPathDirection,
    PathFillType = luanet.SkiaSharp.SKPathFillType
}
";
        private readonly int defaultSize;
        private SKBitmap bitmap;
        private SKCanvas canvas;
        private bool isDrawing = false;
        private bool isCancellationRequested = false;
        public LuaCanvas(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Canvas size must be greater than zero.");
            }
            defaultSize = size;
            bitmap = new SKBitmap(size, size);
            canvas = new SKCanvas(bitmap);
        }
        private void Render()
        {
            IntPtr pixels = bitmap.GetPixels();
            int byteCount = bitmap.ByteCount;
            int width = bitmap.Width;
            int height = bitmap.Height;

            byte[] pixelData = new byte[byteCount];
            Marshal.Copy(pixels, pixelData, 0, byteCount);
            Task.Run(() =>
            {
                EventAggregator.PublishUIDrawMessageCanvas(width, height, pixelData);
            });
        }

        public struct DrawBlueprint
        {
            public LuaFunction? OnInit { get; set; }
            public LuaFunction? OnUpdate { get; set; }
        }

        /*
--- @class Rotoris.LuaCanvas.DrawBlueprint
--- @field OnInit fun(ctx: Rotoris.LuaCanvas.CanvasContext, args: Rotoris.LuaCanvas.CanvasContext.Args): any Initializes the drawing context. Returns an optional state object.
--- @field OnUpdate? fun(ctx: Rotoris.LuaCanvas.CanvasContext, args: Rotoris.LuaCanvas.CanvasContext.Args, state: any) Updates the drawing context. Called repeatedly until done.
--- @field OnFrameDelay? fun(updateTime:number) Called after each successful frame update (`OnUpdate`). Useful for implementing frame rate limiting or delays.
         */
        public void draw(LuaTable table)
        {
            if (isDrawing)
            {
                return;
            }
            isDrawing = true;
            isCancellationRequested = false;

            canvas.Clear(SKColors.Transparent);

            var args = new CanvasContext.Args
            {
                CanvasWidth = bitmap.Width,
                CanvasHeight = bitmap.Height,
                DeltaTime = 0
            };

            using var ctx = new CanvasContext(canvas);
            object? state = null;

            if (table["OnInit"] is LuaFunction init)
            {
                object[] results = init.Call(ctx, args);
                if (results.Length > 0 && results[0] != null)
                {
                    state = results[0];
                }
                Render();
            }

            if (table["OnUpdate"] is LuaFunction update && !ctx.IsDone)
            {
                var onFrameDelay = table["OnFrameDelay"] as LuaFunction;
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                try
                {
                    var onUpdate = update.Call;
                    while (!ctx.IsDone && !isCancellationRequested)
                    {
                        stopwatch.Restart();
                        onUpdate(ctx, args, state);
                        Render();

                        stopwatch.Stop();
                        double updateTime = stopwatch.Elapsed.TotalSeconds;

                        stopwatch.Restart();

                        onFrameDelay?.Call(updateTime);

                        stopwatch.Stop();

                        double delayTime = stopwatch.Elapsed.TotalSeconds;

                        args.DeltaTime = updateTime + delayTime;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"CRITICAL: An exception occurred during the Lua update loop. Loop terminated. Error: {ex.Message}");
                }
                finally
                {
                    stopwatch.Stop();
                }
            }
            isDrawing = false;
        }
        public void set_size(int width = 0, int height = 0)
        {
            if (isDrawing)
            {
                return;
            }
            if (width == 0 && height == 0)
            {
                width = defaultSize;
                height = defaultSize;
            }

            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Canvas dimensions must be greater than zero.");
            }

            if (width == bitmap.Width && height == bitmap.Height) return;

            canvas.Dispose();
            bitmap.Dispose();

            bitmap = new SKBitmap(width, height);
            canvas = new SKCanvas(bitmap);
        }
        public void clear()
        {
            if (isDrawing)
            {
                return;
            }
            canvas.Clear(SKColors.Transparent);
            EventAggregator.PublishUIClearMessageCanvas();
        }
        public void Pause()
        {
            isCancellationRequested = true;
        }
        public void Dispose()
        {
            isCancellationRequested = true;
            canvas.Dispose();
            bitmap.Dispose();
        }
    }

}