namespace RotorisLib.Tests
{
    public class TextImageGeneratorTests
    {
        [Fact]
        public void ToPng_WithValidText_CreatesFile()
        {
            var generator = new TextImageGenerator();
            string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "RotorisTest.png");

            try
            {
                generator.ToPng("test", tempFile);

                Assert.True(System.IO.File.Exists(tempFile), "The PNG file should have been created.");

                var fileInfo = new System.IO.FileInfo(tempFile);
                Assert.True(fileInfo.Length > 0, "The PNG file should not be empty.");
            }
            finally
            {
                generator.Dispose();
                if (System.IO.File.Exists(tempFile))
                {
                    System.IO.File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public void ToBitmapImage_WithValidText_ReturnsValidBitmapImage()
        {
            using var generator = new TextImageGenerator();

            var image = generator.ToBitmapImage("Test");

            Assert.NotNull(image);
            Assert.True(image.PixelWidth > 0);
            Assert.True(image.PixelHeight > 0);
        }

        [Fact]
        public void ToBitmapImage_WithDifferentTexts_ReturnsDifferentWidths()
        {
            using var generator = new TextImageGenerator();

            var narrowImage = generator.ToBitmapImage("i");
            var wideImage = generator.ToBitmapImage("MMMM");

            Assert.NotNull(narrowImage);
            Assert.NotNull(wideImage);
            Assert.True(wideImage.PixelWidth > narrowImage.PixelWidth, "Wider text should produce a wider image.");
        }
    }
}