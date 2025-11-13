using SkiaSharp;

namespace Rotoris.LuaModules.LuaCanvas
{
    /*
--- @class Rotoris.LuaCanvas.FontCache
--- @field load_from_file fun(self: Rotoris.LuaCanvas.FontCache, familyName: string, filePath: string) Loads a font from the specified file and associates it with the given family name.
--- @field get fun(self: Rotoris.LuaCanvas.FontCache, familyName: string, fontSize: number): SkiaSharp.SKFont Retrieves the font associated with the given family name and font size, creating it if it does not exist.
--- @field dispose fun(self: Rotoris.LuaCanvas.FontCache, familyName: string, fontSize: number): boolean Disposes the font associated with the given family name and font size.
--- @field dispose_by_family fun(self: Rotoris.LuaCanvas.FontCache, familyName: string): boolean Disposes all fonts and the typeface associated with the given family name.
     */
    public class FontCache
    {
        private readonly Dictionary<string, SKTypeface> typefaces = [];
        private readonly Dictionary<(string, int), SKFont> fonts = [];
        private SKTypeface GetTypeface(string familyName)
        {
            if (typefaces.TryGetValue(familyName, out var typeface))
            {
                return typeface;
            }
            typeface = SKTypeface.FromFamilyName(familyName);
            typefaces.Add(familyName, typeface);
            return typeface;
        }

        public SKFont Get(string familyName, int fontSize)
        {
            var key = (familyName, fontSize);
            if (fonts.TryGetValue(key, out var font))
            {
                return font;
            }
            var typeface = GetTypeface(familyName);
            font = new SKFont(typeface, fontSize);
            fonts.Add(key, font);
            return font;
        }
        public void load_from_file(string familyName, string filePath)
        {
            SKTypeface typeface = SKTypeface.FromFile(filePath);
            typefaces.Add(familyName, typeface);
        }
        public bool dispose(string familyName, int fontSize)
        {
            return fonts.Remove((familyName, fontSize));
        }
        public bool dispose_by_family(string familyName)
        {
            var fontsToRemove = fonts.Where(kvp => kvp.Key.Item1 == familyName).ToList();

            foreach (var kvp in fontsToRemove)
            {
                kvp.Value.Dispose();
                fonts.Remove(kvp.Key);
            }

            if (typefaces.TryGetValue(familyName, out var typeface))
            {
                typeface.Dispose();
                return typefaces.Remove(familyName);
            }

            return false;
        }
        public void Dispose()
        {
            foreach (var typeface in typefaces.Values)
            {
                typeface.Dispose();
            }
            typefaces.Clear();

            foreach (var font in fonts.Values)
            {
                font.Dispose();
            }
            fonts.Clear();
        }
    }
}
