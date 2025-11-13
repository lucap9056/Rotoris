using NLua;
using SkiaSharp;
using System.Drawing;

namespace Rotoris.LuaModules.LuaCanvas
{
    /*
--- @class Rotoris.LuaCanvas.PaintCache
--- @field create fun(self: Rotoris.LuaCanvas.PaintCache, name: string, table: table): SkiaSharp.SKPaint Creates or retrieves a cached SKPaint object based on the provided name and configuration table.
--- @field set fun(self: Rotoris.LuaCanvas.PaintCache, paint: SkiaSharp.SKPaint) Sets the current SKPaint to be used for drawing operations.
--- @field set fun(self: Rotoris.LuaCanvas.PaintCache, name: string) Sets the current SKPaint to the named paint from the cache.
--- @field update fun(self: Rotoris.LuaCanvas.PaintCache, action: fun(editor: Rotoris.LuaCanvas.Paint.Editor)) Updates the current SKPaint using the provided action function that receives a Paint.Editor.
--- @field update fun(self: Rotoris.LuaCanvas.PaintCache, name: string, action: fun(editor: Rotoris.LuaCanvas.Paint.Editor)): boolean Updates a named SKPaint in the cache using the provided action function that receives a Paint.Editor. Returns true if the paint was found and updated, false otherwise.
--- @field get fun(self: Rotoris.LuaCanvas.PaintCache, name: string?): SkiaSharp.SKPaint Retrieves the current SKPaint or a named SKPaint from the cache.
--- @field dispose fun(self: Rotoris.LuaCanvas.PaintCache, name: string) Disposes and removes a named SKPaint from the cache.
--- @field dispose fun(self: Rotoris.LuaCanvas.PaintCache) Disposes all cached SKPaint objects and clears the cache.
     */
    /// <summary>
    /// Caches and manages SKPaint objects, allowing them to be created and reused 
    /// based on a name and configuration provided from a Lua table.
    /// Implements IDisposable to ensure all cached SKPaint objects are properly disposed.
    /// </summary>
    public class PaintCache : IDisposable
    {
        /// <summary>
        /// Creates an SKPaint object and configures its properties based on key-value pairs 
        /// found in the provided Lua table. Handles type conversions from Lua's common 
        /// return types (e.g., double for enums, string for color) to C# types.
        /// </summary>
        /// <param name="table">The LuaTable containing paint configuration properties.</param>
        /// <returns>A new, configured SKPaint instance.</returns>
        private static SKPaint PaintFromTable(LuaTable table)
        {
            SKPaint paint = new();

            try
            {
                if (table["Color"] is string hexColor)
                {
                    Color color = ColorTranslator.FromHtml(hexColor);
                    paint.Color = new SKColor(color.R, color.G, color.B, color.A);
                }

                if (table["IsAntialias"] is bool isAntialias)
                {
                    paint.IsAntialias = isAntialias;
                }

                if (table["IsDither"] is bool isDither)
                {
                    paint.IsDither = isDither;
                }

                if (table["Style"] != null)
                {
                    if (table["Style"] is double styleDouble)
                    {
                        paint.Style = (SKPaintStyle)(int)styleDouble;
                    }

                    else if (table["Style"] is SKPaintStyle style)
                    {
                        paint.Style = style;
                    }
                }

                if (table["BlendMode"] != null)
                {
                    if (table["BlendMode"] is double modeDouble)
                    {
                        paint.BlendMode = (SKBlendMode)(int)modeDouble;
                    }
                    else if (table["BlendMode"] is SKBlendMode mode)
                    {
                        paint.BlendMode = mode;
                    }
                }


                if (table["StrokeWidth"] != null)
                {
                    paint.StrokeWidth = Convert.ToSingle(table["StrokeWidth"]);
                }

                if (table["StrokeCap"] != null)
                {
                    if (table["StrokeCap"] is double capDouble)
                    {
                        paint.StrokeCap = (SKStrokeCap)(int)capDouble;
                    }
                    else if (table["StrokeCap"] is SKStrokeCap cap)
                    {
                        paint.StrokeCap = cap;
                    }
                }

                if (table["StrokeJoin"] != null)
                {
                    if (table["StrokeJoin"] is double joinDouble)
                    {
                        paint.StrokeJoin = (SKStrokeJoin)(int)joinDouble;
                    }
                    else if (table["StrokeJoin"] is SKStrokeJoin join)
                    {
                        paint.StrokeJoin = join;
                    }
                }

                if (table["StrokeMiter"] != null)
                {
                    paint.StrokeMiter = Convert.ToSingle(table["StrokeMiter"]);
                }

                if (table["PathEffect"] is SKPathEffect pathEffect)
                {
                    paint.PathEffect = pathEffect;
                }

                if (table["Shader"] is SKShader shader)
                {
                    paint.Shader = shader;
                }

                return paint;
            }
            catch (Exception ex)
            {
                paint.Dispose();
                throw new ArgumentException($"Failed to create SKPaint from Lua table: {ex.Message}", ex);
            }
        }

        private static readonly SKPaint DefaultPaint = new()
        {
            Color = new SKColor(0, 0, 0)
        };

        private readonly Dictionary<string, SKPaint> paints = [];
        private readonly Lock paintsLock = new();
        private SKPaint currentPaint = DefaultPaint;

        public SKPaint create(string name, LuaTable table)
        {
            lock (paintsLock)
            {
                if (paints.TryGetValue(name, out SKPaint? value))
                {
                    return value;
                }
                SKPaint paint = PaintFromTable(table);
                paints.Add(name, paint);
                return paint;
            }
        }

        public void set(SKPaint paint)
        {
            lock (paintsLock)
            {
                currentPaint = paint;
            }
        }

        public void set(string name)
        {
            lock (paintsLock)
            {
                if (paints.TryGetValue(name, out SKPaint? paint))
                {
                    currentPaint = paint;
                }
            }
        }

        public void update(LuaFunction action)
        {
            lock (paintsLock)
            {
                PaintEditor editor = new(currentPaint);
                action.Call(editor);
            }
        }

        public bool update(string name, LuaFunction action)
        {
            lock (paintsLock)
            {
                if (paints.TryGetValue(name, out var paint))
                {
                    PaintEditor editor = new(paint);
                    action.Call(editor);
                    return true;
                }
                return false;
            }
        }

        public SKPaint get(string? name = null)
        {
            lock (paintsLock)
            {
                if (!string.IsNullOrEmpty(name) && paints.TryGetValue(name, out SKPaint? paint))
                {
                    return paint;
                }
                return currentPaint;
            }
        }

        public void dispose(string name)
        {
            lock (paintsLock)
            {
                if (paints.TryGetValue(name, out SKPaint? paint))
                {
                    paints.Remove(name);
                    paint.Dispose();
                }
            }
        }

        public void Dispose()
        {
            currentPaint = DefaultPaint;
            lock (paintsLock)
            {
                foreach (var paint in paints.Values)
                {
                    paint.Dispose();
                }
                paints.Clear();
            }
        }
    }

    /*
--- @class Rotoris.LuaCanvas.PaintCache.Editor
--- @field set_color fun(self: Rotoris.LuaCanvas.Paint.Editor, hexColor: string) Sets the color of the paint using a hex color string (e.g., "#RRGGBB" or "#AARRGGBB").
--- @field set_style fun(self: Rotoris.LuaCanvas.Paint.Editor, style: SkiaSharp.SKPaintStyle) Sets the style of the paint.
--- @field set_blend_mode fun(self: Rotoris.LuaCanvas.Paint.Editor, mode: SkiaSharp.SKBlendMode) Sets the blend mode of the paint.
--- @field set_antialias fun(self: Rotoris.LuaCanvas.Paint.Editor, enabled: boolean) Enables or disables antialiasing for the paint.
--- @field set_dither fun(self: Rotoris.LuaCanvas.Paint.Editor, enabled: boolean) Enables or disables dithering for the paint.
--- @field set_stroke_width fun(self: Rotoris.LuaCanvas.Paint.Editor, width: number) Sets the stroke width of the paint.
--- @field set_stroke_cap fun(self: Rotoris.LuaCanvas.Paint.Editor, cap: SkiaSharp.SKStrokeCap) Sets the stroke cap of the paint.
--- @field set_stroke_join fun(self: Rotoris.LuaCanvas.Paint.Editor, join: SkiaSharp.SKStrokeJoin) Sets the stroke join of the paint.
--- @field set_stroke_miter fun(self: Rotoris.LuaCanvas.Paint.Editor, miter: number) Sets the stroke miter of the paint.
--- @field set_path_effect fun(self: Rotoris.LuaCanvas.Paint.Editor, effect: SkiaSharp.SKPathEffect?) Sets the path effect of the paint.
--- @field set_shader fun(self: Rotoris.LuaCanvas.Paint.Editor, shader: SkiaSharp.SKShader?) Sets the shader of the paint.
     */
    public class PaintEditor(SKPaint p)
    {
        private readonly SKPaint paint = p;

        public void set_color(string hexColor)
        {
            if (!string.IsNullOrEmpty(hexColor))
            {
                try
                {
                    System.Drawing.Color color = ColorTranslator.FromHtml(hexColor);
                    paint.Color = new SKColor(color.R, color.G, color.B, color.A);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Invalid color string '{hexColor}'. Error: {ex.Message}");
                }
            }
        }

        public void set_style(SKPaintStyle style)
        {
            paint.Style = style;
        }

        public void set_blend_mode(SKBlendMode mode)
        {
            paint.BlendMode = mode;
        }

        public void set_antialias(bool enabled)
        {
            paint.IsAntialias = enabled;
        }

        public void set_dither(bool enabled)
        {
            paint.IsDither = enabled;
        }

        public void set_stroke_width(float width)
        {
            paint.StrokeWidth = width;
        }

        public void set_stroke_cap(SKStrokeCap cap)
        {
            paint.StrokeCap = cap;
        }

        public void set_stroke_join(SKStrokeJoin join)
        {
            paint.StrokeJoin = join;
        }

        public void set_stroke_miter(float miter)
        {
            paint.StrokeMiter = miter;
        }

        public void set_path_effect(SKPathEffect? effect)
        {
            paint.PathEffect = effect;
        }

        public void set_shader(SKShader? shader)
        {
            paint.Shader = shader;
        }
    }
}