using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{
    public class AlbumCoverParams
    {
        public required string Title { get; set; }
        public required string Artist { get; set; }
        public required string Album { get; set; }
    }
    public static class AlbumCover
    {
        private const int CanvasSize = 1000;

        public static string Make(AlbumCoverParams parameters, Random seed)
        {
            var bitmap = DrawOnCanvas(parameters, seed);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            byte[] pngBytes = data.ToArray();
            return Convert.ToBase64String(pngBytes);
        }

        public static SKBitmap DrawOnCanvas(AlbumCoverParams parameters, Random seed)
        {
            var bitmap = new SKBitmap(CanvasSize, CanvasSize, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bitmap);
            Background.Draw(canvas);
            canvas.Flush();
            return bitmap;
        }

    }
}