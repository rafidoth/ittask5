
using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{
    public static class Background
    {
        public static void Draw(SKCanvas canvas)
        {
            var baseColor = SKColor.FromHsl(200, 50, 50);
            FillWithBaseColor(canvas, baseColor);
        }
        private static void FillWithBaseColor(SKCanvas canvas, SKColor color)
        {
            canvas.Clear(color);
        }
    }
}