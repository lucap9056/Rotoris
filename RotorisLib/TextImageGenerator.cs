namespace RotorisLib
{

    /// <summary>
    /// A class for generating image files from text using SkiaSharp.
    /// </summary>
    public class TextImageGenerator : System.IDisposable
    {
        private readonly SkiaSharp.SKTypeface typeface = SkiaSharp.SKTypeface.Default;
        private readonly SkiaSharp.SKFont font;
        private readonly SkiaSharp.SKPaint fillPaint;
        private readonly SkiaSharp.SKPaint strokePaint;

        /// <summary>Initializes a new instance of the <see cref="TextImageGenerator"/> class.</summary>
        public TextImageGenerator()
        {
            font = new(typeface, 64);

            fillPaint = new()
            {
                Color = SkiaSharp.SKColors.Black,
                IsAntialias = true,
                Style = SkiaSharp.SKPaintStyle.Fill,
            };

            strokePaint = new()
            {
                Color = SkiaSharp.SKColors.White,
                IsAntialias = true,
                Style = SkiaSharp.SKPaintStyle.Stroke,
                StrokeWidth = 10,
                StrokeJoin = SkiaSharp.SKStrokeJoin.Round
            };
        }

        /// <summary>
        /// Generates a bitmap containing the specified text.
        /// </summary>
        /// <param name="text">The text string to render.</param>
        /// <returns>A new <see cref="SkiaSharp.SKBitmap"/> containing the rendered text.</returns>
        private SkiaSharp.SKBitmap GetBitmap(string text)
        {
            font.MeasureText(text, out SkiaSharp.SKRect textBounds);
            font.GetFontMetrics(out SkiaSharp.SKFontMetrics metrics);

            float strokeExtra = strokePaint.StrokeWidth * 2;
            int width = (int)System.Math.Ceiling(textBounds.Width + strokeExtra);
            int height = (int)System.Math.Ceiling(metrics.Descent - metrics.Ascent + strokeExtra);

            float textWidth = textBounds.Width;

            SkiaSharp.SKBitmap bitmap = new(width, height);
            using SkiaSharp.SKCanvas canvas = new(bitmap);
            canvas.Clear(SkiaSharp.SKColors.Transparent);

            float offsetX = strokePaint.StrokeWidth;
            // Center the text vertically. The y-coordinate is the baseline.
            float y = offsetX + (height - strokeExtra) / 2f - (metrics.Ascent + metrics.Descent) / 2f;
            // Center the text horizontally.
            float x = offsetX + (width - strokeExtra) / 2f - (textWidth / 2f) - textBounds.Left;

            using SkiaSharp.SKTextBlob? textBlob = SkiaSharp.SKTextBlob.Create(text, font);

            // Draw stroke first for outline effect
            canvas.DrawText(textBlob, x, y, strokePaint);
            // Draw fill second
            canvas.DrawText(textBlob, x, y, fillPaint);

            return bitmap;
        }

        /// <summary>
        /// Renders text to a PNG file at the specified output path.
        /// </summary>
        /// <param name="text">The text string to render.</param>
        /// <param name="outputPath">The file path where the PNG image will be saved.</param>
        public void ToPng(string text, string outputPath)
        {
            using var bitmap = GetBitmap(text);
            using SkiaSharp.SKData data = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);

            // Ensure directory exists before writing file
            string? directory = System.IO.Path.GetDirectoryName(outputPath);
            if (directory != null)
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            using System.IO.FileStream stream = System.IO.File.OpenWrite(outputPath);
            data.SaveTo(stream);
        }

        /// <summary>
        /// Renders text to a <see cref="System.Windows.Media.Imaging.BitmapImage"/> for WPF usage.
        /// </summary>
        /// <param name="text">The text string to render.</param>
        /// <returns>A WPF <see cref="System.Windows.Media.Imaging.BitmapImage"/>.</returns>
        public System.Windows.Media.Imaging.BitmapImage ToBitmapImage(string text)
        {
            using var bitmap = GetBitmap(text);
            using var stream = new System.IO.MemoryStream();
            bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100).SaveTo(stream);
            stream.Position = 0;

            System.Windows.Media.Imaging.BitmapImage image = new();
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }

        /// <summary>Disposes of managed resources used by the renderer.</summary>
        public void Dispose()
        {
            font.Dispose();
            typeface.Dispose();
            fillPaint.Dispose();
            strokePaint.Dispose();
            // Suppress finalization.
            System.GC.SuppressFinalize(this);
        }
    }
}
