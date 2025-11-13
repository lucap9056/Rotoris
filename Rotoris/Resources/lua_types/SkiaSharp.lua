--- @enum SkiaSharp.SKPaintStyle
local SKPaintStyle = {
    Fill = 0,          --- Fills the shape.
    Stroke = 1,        --- Strokes the shape's outline.
    StrokeAndFill = 2, --- Strokes and fills the shape.
}

--- @enum SkiaSharp.SKTextAlign
local SKTextAlign = {
    Left = 0,   --- Aligns text to the left of the position.
    Center = 1, --- Centers text on the position.
    Right = 2,  --- Aligns text to the right of the position.
}

--- @enum SkiaSharp.SKStrokeCap
local SKStrokeCap = {
    Butt = 0,   --- No extra cap is added.
    Round = 1,  --- A semicircle is added.
    Square = 2, --- A square is added.
}

--- @enum SkiaSharp.SKStrokeJoin
local SKStrokeJoin = {
    Miter = 0, --- Sharp corners are used.
    Round = 1, --- Rounded corners are used.
    Bevel = 2, --- Flat corners are used.
}

--- @enum SkiaSharp.SKBlendMode
local SKBlendMode = {
    Clear = 0,       --- Clears the destination color.
    Src = 1,         --- Uses the source color.
    Dst = 2,         --- Uses the destination color.
    SrcOver = 3,     --- Source over destination.
    DstOver = 4,     --- Destination over source.
    SrcIn = 5,       --- Source inside destination.
    DstIn = 6,       --- Destination inside source.
    SrcOut = 7,      --- Source outside destination.
    DstOut = 8,      --- Destination outside source.
    SrcATop = 9,     --- Source over destination, but only where destination is defined.
    DstATop = 10,    --- Destination over source, but only where source is defined.
    Xor = 11,        --- Exclusive OR.
    Plus = 12,       --- Adds the source and destination colors.
    Modulate = 13,   --- Multiplies source and destination colors.
    Screen = 14,     --- Multiplies the inverse of the colors.
    Overlay = 15,    --- Multiplies or screens colors, depending on the destination color.
    Darken = 16,     --- Selects the darker color component.
    Lighten = 17,    --- Selects the lighter color component.
    ColorDodge = 18, --- Brightens the destination color to reflect the source.
    ColorBurn = 19,  --- Darkens the destination color to reflect the source.
    HardLight = 20,  --- Multiplies or screens colors, depending on the source color.
    SoftLight = 21,  --- Darkens or lightens colors, depending on the source color.
    Difference = 22, --- Subtracts the darker color from the lighter color.
    Exclusion = 23,  --- Produces a result similar to Difference, but softer.
    Multiply = 24,   --- Multiplies the colors.
    Hue = 25,        --- Uses the source hue with the destination's saturation and luminosity.
    Saturation = 26, --- Uses the source saturation with the destination's hue and luminosity.
    Color = 27,      --- Uses the source hue and saturation with the destination's luminosity.
    Luminosity = 28, --- Uses the source luminosity with the destination's hue and saturation.
}


--- Creates a path effect that 'dashes' a path according to the intervals, and shifts the start of the pattern by the phase.
--- The result is an effect that inverts the geometry of the path, converting solid lines to dashed lines, etc.
--- @param intervals number[] An array of numbers specifying the lengths of the "on" and "off" intervals for the dash pattern. Must have an even number of entries (>= 2).
--- @param phase number The distance into the interval array at which to start the dashing.
--- @return SkiaSharp.SKPathEffect
local function CreateDash(intervals, phase) return {} end

--- Creates a path effect that breaks the path into discrete segments of the same length, with random displacement.
--- @param segmentLength number The length of the segments. Must be positive.
--- @param deviation number The maximum distance to displace the segment. Must be non-negative.
--- @return SkiaSharp.SKPathEffect
local function CreateDiscrete(segmentLength, deviation) return {} end

--- Creates a path effect that trims a path to a specific range.
--- @param start number The starting point on the path (0.0 to 1.0).
--- @param stop number The stopping point on the path (0.0 to 1.0).
--- @param mode SkiaSharp.SKPathEffectTrimMode The mode used to determine which part of the path to keep. (Defaults to 'Normal')
--- @return SkiaSharp.SKPathEffect
local function CreateTrim(start, stop, mode) return {} end

--- Path effects are used to modify the geometry of a path before it is drawn.
--- @class SkiaSharp.SKPathEffect
local SKPathEffect = {
    CreateDash = CreateDash,
    CreateDiscrete = CreateDiscrete,
    CreateTrim = CreateTrim,
}

--- Creates a shader that linearly interpolates colors between two or more points.
--- @param points SkiaSharp.SKPoint[] The start and end points of the gradient (must be exactly two points).
--- @param colors string[]|number[] An array of colors (e.g., "#AARRGGBB" hex strings or 32-bit integers).
--- @param colorPositions number[]|nil An optional array of color stop positions (0.0 to 1.0). The array length must match the number of colors.
--- @param tileMode SkiaSharp.SKShaderTileMode The tiling mode. (Defaults to 'Clamp')
--- @param matrix SkiaSharp.SKMatrix|nil The local matrix for the shader.
--- @return SkiaSharp.SKShader
local function CreateLinearGradient(points, colors, colorPositions, tileMode, matrix) return {} end

--- Creates a shader that radially interpolates colors from a central point.
--- @param center SkiaSharp.SKPoint The center point of the gradient.
--- @param radius number The radius of the gradient. Must be non-negative.
--- @param colors string[]|number[] An array of colors (e.g., "#AARRGGBB" hex strings or 32-bit integers).
--- @param colorPositions number[]|nil An optional array of color stop positions (0.0 to 1.0). The array length must match the number of colors.
--- @param tileMode SkiaSharp.SKShaderTileMode The tiling mode. (Defaults to 'Clamp')
--- @param matrix SkiaSharp.SKMatrix|nil The local matrix for the shader.
--- @return SkiaSharp.SKShader
local function CreateRadialGradient(center, radius, colors, colorPositions, tileMode, matrix) return {} end

--- Creates a shader that sweeps colors around a center point.
--- @param center SkiaSharp.SKPoint The center point of the sweep.
--- @param startAngle number The starting angle (degrees).
--- @param endAngle number The ending angle (degrees).
--- @param colors string[]|number[] An array of colors (e.g., "#AARRGGBB" hex strings or 32-bit integers).
--- @param colorPositions number[]|nil An optional array of color stop positions (0.0 to 1.0). The array length must match the number of colors.
--- @param tileMode SkiaSharp.SKShaderTileMode The tiling mode. (Defaults to 'Clamp')
--- @param matrix SkiaSharp.SKMatrix|nil The local matrix for the shader.
--- @return SkiaSharp.SKShader
local function CreateSweepGradient(center, startAngle, endAngle, colors, colorPositions, tileMode, matrix) return {} end

--- Creates a shader that draws a bitmap.
--- @param bitmap SkiaSharp.SKBitmap The bitmap to use as a texture. Must be a valid SKBitmap object.
--- @param tileX SkiaSharp.SKShaderTileMode The tiling mode for the X-axis. (Defaults to 'Clamp')
--- @param tileY SkiaSharp.SKShaderTileMode The tiling mode for the Y-axis. (Defaults to 'Clamp')
--- @param matrix SkiaSharp.SKMatrix|nil The local matrix for the shader.
--- @return SkiaSharp.SKShader
local function CreateBitmap(bitmap, tileX, tileY, matrix) return {} end

--- Shaders are used to describe how colors or patterns are applied to a shape.
--- @class SkiaSharp.SKShader
local SKShader = {
    CreateLinearGradient = CreateLinearGradient,
    CreateRadialGradient = CreateRadialGradient,
    CreateSweepGradient = CreateSweepGradient,
    CreateBitmap = CreateBitmap
}

--- @class SkiaSharp.SKPaint
local SKPaint = {
    -- Properties
    Color = '',                      --- @type string The paint's color, as a hex string (e.g., "#AARRGGBB").
    Style = SKPaintStyle.Fill,       --- @type number The paint's style (Fill, Stroke, or StrokeAndFill).
    StrokeWidth = 1,                 --- @type number The thickness of the stroke. Must be non-negative.
    StrokeCap = SKStrokeCap.Butt,    --- @type number The treatment applied to the end of a line.
    StrokeJoin = SKStrokeJoin.Miter, --- @type number The treatment applied to the corners of a line.
    IsAntialias = true,              --- @type boolean Whether anti-aliasing is enabled for smoothing edges.
    TextSize = 12,                   --- @type number The size of the text to be drawn. Must be positive.
    TextAlign = SKTextAlign.Left,    --- @type number How the text is aligned (Left, Center, Right).
    Shader = nil,                    --- @type SkiaSharp.SKShader|nil The shader to use for drawing colors or patterns.
    PathEffect = nil,                --- @type SkiaSharp.SKPathEffect|nil The path effect to apply to the geometry.
    BlendMode = SKBlendMode.SrcOver, --- @type number The blending mode for combining source and destination colors.

    -- Methods
    Dispose = function() end, --- Releases the native resources used by the SKPaint object.
}

--- @class SkiaSharp.SKPathEffectTrimMode
local SKPathEffectTrimMode = {
    Normal = 0,   --- Keeps the segment from start to stop.
    Inverted = 1, --- Keeps the segment from stop to start (the remainder of the path).
}

--- @class SkiaSharp.SKPoint
local SKPoint = {
    X = 0, --- @type number The X-coordinate.
    Y = 0, --- @type number The Y-coordinate.

    -- Static Method
    Create = function(x, y) return { X = x, Y = y } end, --- @param x number The X-coordinate. @param y number The Y-coordinate. @return SkiaSharp.SKPoint A new SKPoint object.
}

--- @class SkiaSharp.SKShaderTileMode
local SKShaderTileMode = {
    Clamp = 0,  --- Extends the edge color or image.
    Repeat = 1, --- Repeats the pattern horizontally and vertically.
    Mirror = 2, --- Repeats the pattern, flipping it on alternating repeats.
}

--- @class SkiaSharp.SKMatrix
local SKMatrix = {
    ScaleX = 1, --- @type number X-axis scale component (M00).
    SkewX = 0,  --- @type number X-axis skew component (M01).
    TransX = 0, --- @type number X-axis translation component (M02).
    SkewY = 0,  --- @type number Y-axis skew component (M10).
    ScaleY = 1, --- @type number Y-axis scale component (M11).
    TransY = 0, --- @type number Y-axis translation component (M12).
    Persp0 = 0, --- @type number Perspective component (M20).
    Persp1 = 0, --- @type number Perspective component (M21).
    Persp2 = 1, --- @type number Perspective component (M22).

    -- Static Methods
    CreateScale = function(sx, sy) return {} end,             --- @param sx number The scale factor for the X-axis. @param sy number The scale factor for the Y-axis. @return SkiaSharp.SKMatrix Creates a scaling matrix.
    CreateTranslation = function(dx, dy) return {} end,       --- @param dx number The translation delta for the X-axis. @param dy number The translation delta for the Y-axis. @return SkiaSharp.SKMatrix Creates a translation matrix.
    CreateRotation = function(degrees, px, py) return {} end, --- @param degrees number The rotation angle in degrees. @param px number The X-coordinate of the rotation center. @param py number The Y-coordinate of the rotation center. @return SkiaSharp.SKMatrix Creates a rotation matrix around a specified point.
    Concat = function(a, b) return {} end,                    --- @param a SkiaSharp.SKMatrix The first matrix. @param b SkiaSharp.SKMatrix The second matrix. @return SkiaSharp.SKMatrix Concatenates two matrices (a * b).
}

--- @class SkiaSharp.SKBitmap
local SKBitmap = {
    Width = 0,  --- @type number The width of the bitmap in pixels.
    Height = 0, --- @type number The height of the bitmap in pixels.

    -- Methods
    Dispose = function() end,                       --- Releases the native resources used by the SKBitmap object.
    -- Static Method
    Decode = function(data) return {} end,          --- @param data any Image data (e.g., file path or byte array). @return SkiaSharp.SKBitmap|nil Decodes an image from data. Returns nil on failure.
    Create = function(width, height) return {} end, --- @param width number The width of the new bitmap. @param height number The height of the new bitmap. @return SkiaSharp.SKBitmap Creates a new blank bitmap.
}

--- @enum SkiaSharp.SKPathDirection
local SKPathDirection = {
    Clockwise = 0,       --- Specifies a clockwise path winding. (���w���ɰw�����|��¶��V�C)
    CounterClockwise = 1 --- Specifies a counter-clockwise path winding. (���w�f�ɰw�����|��¶��V�C)
}

--- @enum SkiaSharp.SKPathFillType
local SKPathFillType = {
    Winding        = 0, --- The default filling mode: determines whether a point is inside a path by adding up the path's winding number. (�w�]��R�Ҧ��G�z�L�p����|��¶�ƨӧP�_�I�O�_�b���|���C)
    EvenOdd        = 1, --- Determines whether a point is inside a path by counting the number of times a ray from the point intersects the path. (�z�L�p��g�u�P���|�����I�ƨӧP�_�I�O�_�b���|���A�_�Ʀb���A���Ʀb�~�C)
    InverseWinding = 2, --- Inverts the Winding rule. (���� Winding �W�h�C)
    InverseEvenOdd = 3  --- Inverts the EvenOdd rule. (���� EvenOdd �W�h�C)
}

--- Represents a geometric path for drawing.
--- @class SkiaSharp.SKPath
local SKPath = {}

--- Represents an axis-aligned rectangle.
--- @class SkiaSharp.SKRect
local SKRect = {
    Top = 0,                          --- @type number The Y-coordinate of the top edge.
    Left = 0,                         --- @type number The X-coordinate of the left edge.
    Right = 0,                        --- @type number The X-coordinate of the right edge.
    Bottom = 0,                       --- @type number The Y-coordinate of the bottom edge.
    Width = 0,                        --- @type number The width of the rectangle (Right - Left).
    Height = 0,                       --- @type number The height of the rectangle (Bottom - Top).
    Location = { X = 0, Y = 0 },      --- @type SkiaSharp.SKPoint The top-left corner (Left, Top).
    MidX = 0,                         --- @type number The X-coordinate of the center.
    MidY = 0,                         --- @type number The Y-coordinate of the center.
    Size = { Width = 0, Height = 0 }, --- @type table The dimensions (Width, Height).
    Standardized = true               --- @type boolean True if Left <= Right and Top <= Bottom.
}

--- @enum SkiaSharp.SKColorType
local SKColorType = {
    Unknown = 0,      --- Unknown color type.
    Rgba8888 = 1,     --- 32-bit ARGB with each component being 8 bits.
    Bgra8888 = 2,     --- 32-bit ABGR with each component being 8 bits.
    RgbaFloat128 = 3, --- 128-bit RGBA with each component being a 32-bit float.
}

--- Provides information about an image's dimensions and color type.
--- @class SkiaSharp.SKImageInfo
local SKImageInfo = {
    Width = 0,                       --- @type number The width of the image.
    Height = 0,                      --- @type number The height of the image.
    ColorType = SKColorType.Unknown, --- @type number The color type of the image.

    --- Creates an SKImageInfo object.
    --- @param width number The width of the image.
    --- @param height number The height of the image.
    --- @param colorType number The color type of the image.
    --- @return SkiaSharp.SKImageInfo A new SKImageInfo object.
    Create = function(width, height, colorType)
        return {
            Width = width,
            Height = height,
            ColorType = colorType,
        }
    end,
}

--- @class SkiaSharp.SKFont

--- Generates a hexadecimal color string from RGBA values.
-- The function ensures that the input values are clamped to the valid range of 0-255.
-- If any input is missing, it defaults to 255.
-- @param r number The red component (0-255). Defaults to 255.
-- @param g number The green component (0-255). Defaults to 255.
-- @param b number The blue component (0-255). Defaults to 255.
-- @param a number The alpha component (0-255). Defaults to 255.
-- @return string A hexadecimal color string in the format "#AARRGGBB".
local function color_from_rgba(r, g, b, a) return '' end

return {
    color_from_rgba = color_from_rgba,
    PaintStyle = SKPaintStyle,
    TextAlign = SKTextAlign,
    StrokeCap = SKStrokeCap,
    StrokeJoin = SKStrokeJoin,
    BlendMode = SKBlendMode,
    PathEffect = SKPathEffect,
    Shader = SKShader,
    PathDirection = SKPathDirection,
    PathFillType = SKPathFillType,
}
