using Rotoris.LuaModules.LuaUtils;
using SkiaSharp;
using System.IO;

namespace Rotoris.LuaModules.LuaCanvas
{
    /*
--- @class Rotoris.LuaCanvas.ImageCache
--- @field load_bytes fun(imageId: string, imageData: bytes): SkiaSharp.SKBitmap Loads an image from the provided byte array and caches it with the specified ID.
--- @field get fun(imagePath: string): SkiaSharp.SKBitmap|nil Retrieves the cached image for the specified file path, loading it if not already cached.
--- @field load fun(filePath: string): SkiaSharp.SKImageInfo|nil Loads the image info from the specified file path.
--- @field dispose fun(filePath: string): boolean Disposes of the cached image associated with the specified file path.
     */
    public class ImageCache
    {
        private readonly Dictionary<string, SKBitmap> bitmaps = [];
        public SKBitmap load_bytes(string imageId, Bytes imageData)
        {
            var newBitmap = SKBitmap.Decode(imageData.Data) ??
                throw new InvalidOperationException($"Failed to decode image '{imageId}' from bytes.");
            bitmaps.Add(imageId, newBitmap);
            return newBitmap;
        }
        public SKBitmap? get(string imagePath)
        {

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                throw new ArgumentException("Image path cannot be null or empty.", nameof(imagePath));
            }

            {
                if (bitmaps.TryGetValue(imagePath, out var bitmap))
                {
                    return bitmap;
                }
            }

            string normalizedPath = Path.GetFullPath(Path.Combine(RotorisLib.AppConstants.AppModuleDirectory, imagePath));

            {
                if (bitmaps.TryGetValue(normalizedPath, out var bitmap))
                {
                    return bitmap;
                }
            }

            try
            {
                using var stream = File.OpenRead(normalizedPath);

                var newBitmap = SKBitmap.Decode(stream) ??
                    throw new InvalidOperationException($"Failed to decode image from path: '{normalizedPath}'.");

                bitmaps.Add(normalizedPath, newBitmap);
                return newBitmap;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"The image file was not found at the specified path: '{normalizedPath}'.", normalizedPath, ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while loading the image from path: '{normalizedPath}'. See inner exception for details.", ex);
            }
        }
        public SKImageInfo? load(string filePath)
        {
            return get(filePath)?.Info;
        }
        public bool dispose(string filePath)
        {
            return bitmaps.Remove(filePath);
        }
        public void Dispose()
        {
            foreach (var bitmap in bitmaps.Values)
            {
                bitmap.Dispose();
            }
            bitmaps.Clear();
        }
    }
}
