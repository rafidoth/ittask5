using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{
    public class AlbumCoverParams
    {
        public required string Title { get; set; }
        public required string Artist { get; set; }
        public required string Album { get; set; }
    }

    public class DesignElements
    {
        public bool Overlay { get; set; }
        public TextLayout TextLayout { get; set; }
        public bool DiceBear { get; set; }
    }
    public static class AlbumCover
    {
        private const int CanvasSize = 1000;

        public static async Task<string> Make(AlbumCoverParams parameters, Random seed)
        {
            var bitmap = await DrawOnCanvas(parameters, seed);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            byte[] pngBytes = data.ToArray();
            return Convert.ToBase64String(pngBytes);
        }

        public static async Task<SKBitmap> DrawOnCanvas(AlbumCoverParams parameters, Random seed)
        {
            var bitmap = new SKBitmap(CanvasSize, CanvasSize, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bitmap);
            ColorPalette palette = Colors.GetColorPalette(seed);
            Background.Draw(canvas, CanvasSize, seed, palette);
            await ApplyRandomCombinations(canvas, CanvasSize, palette, parameters, seed);
            canvas.Flush();
            return bitmap;
        }

        private static async Task ApplyRandomCombinations(SKCanvas canvas, int CanvasSize, ColorPalette palette, AlbumCoverParams parameters, Random seed)
        {
            var combinations = GetDesignCombinations();
            var rngCombo = combinations[seed.Next(combinations.Length)];
            await ApplyDesign(canvas, CanvasSize, palette, parameters, seed,
                rngCombo.Overlay, rngCombo.TextLayout, rngCombo.DiceBear);
        }


        private static DesignElements[] GetDesignCombinations()
        {
            var combinations = new List<DesignElements>();
            foreach (var overlay in new[] { true, false })
            {
                foreach (var diceBear in new[] { true, false })
                {
                    foreach (var textLayout in new[]
                    {
                        TextLayout.BothBottom,
                        TextLayout.BothTop,
                        TextLayout.TitleTopSubtitleBottom
                    })
                    {
                        combinations.Add(new DesignElements
                        {
                            Overlay = overlay,
                            TextLayout = textLayout,
                            DiceBear = diceBear
                        });
                    }
                }
            }

            return combinations.ToArray();
        }

        private static async Task ApplyDesign(SKCanvas canvas, int size, ColorPalette palette, AlbumCoverParams parameters, Random seed, bool overlay, TextLayout textLayout, bool diceBear)
        {
            DiceBearVerticalPosition position = textLayout switch
            {
                TextLayout.BothBottom => DiceBearVerticalPosition.Top,
                TextLayout.BothTop => DiceBearVerticalPosition.Bottom,
                _ => DiceBearVerticalPosition.Center
            };
            float iconScale = textLayout == TextLayout.TitleTopSubtitleBottom ? 0.35f : 0.5f;
            if (diceBear) await DiceBear.Draw(canvas, size, seed, iconScale, position);
            if (overlay) Overlay.Apply(canvas, size, seed, palette);
            TextTitle.Draw(canvas, size, palette, parameters.Title, parameters.Artist, textLayout, seed);
        }

    }
}