using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{

    public record ColorPalette(
        SKColor Background,
        SKColor BackgroundAlt
    );
    public static class Colors
    {
        public static SKColor ShiftHue(SKColor color, float degrees)
        {
            color.ToHsl(out float h, out float s, out float l);
            h = ((h + degrees) % 360f + 360f) % 360f;
            return SKColor.FromHsl(h, s, l);
        }
    }
}
