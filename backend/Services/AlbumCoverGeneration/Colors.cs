using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{

    public record ColorPalette(
        SKColor Background,
        SKColor BackgroundAlt,
        SKColor Accent,
        SKColor NoiseLight,
        SKColor NoiseDark,
        SKColor TextTitle,
        SKColor TextSubtitle
    );

    public enum ColorHarmony { Complementary, Analogous, Triadic, SplitComplementary, Tetradic }

    public static class Colors
    {
        public static ColorPalette GetColorPalette(Random rng)
        {
            float baseHue = rng.NextSingle() * 360f;
            var harmony = (ColorHarmony)rng.Next(5);
            float accentHue = baseHue + AccentOffset(harmony, rng);
            bool isDark = rng.NextDouble() < 0.20;
            return BuildPalette(rng, baseHue, accentHue, isDark);
        }

        private static float AccentOffset(ColorHarmony h, Random rng) => h switch
        {
            ColorHarmony.Complementary => 180f,
            ColorHarmony.Analogous => 30f + rng.NextSingle() * 20f,
            ColorHarmony.Triadic => 120f,
            ColorHarmony.SplitComplementary => 150f + rng.NextSingle() * 30f,
            ColorHarmony.Tetradic => 90f,
            _ => 180f
        };

        private static ColorPalette BuildPalette(Random rng, float baseHue, float accentHue, bool isDark)
        {
            float bgLight = BgLightness(isDark, rng);
            float bgSat = BgSaturation(isDark, rng);
            return new ColorPalette(
                Background: SKColor.FromHsl(NormHue(baseHue), bgSat, bgLight),
                BackgroundAlt: SKColor.FromHsl(NormHue(baseHue + 15f), bgSat * 0.7f, bgLight + (isDark ? 8f : -8f)),
                Accent: SKColor.FromHsl(NormHue(accentHue), AccentSaturation(rng), AccentLightness(rng)),
                NoiseLight: SKColor.FromHsl(NormHue(baseHue + 30f), 25f, NoiseLightness(true)),
                NoiseDark: SKColor.FromHsl(NormHue(baseHue + 180f), 35f, NoiseLightness(false)),
                TextTitle: TitleColor(baseHue, isDark),
                TextSubtitle: SubtitleColor(baseHue, isDark)
            );
        }

        private static float BgSaturation(bool isDark, Random rng)
            => isDark ? 30f + rng.NextSingle() * 30f : 20f + rng.NextSingle() * 20f;

        private static float BgLightness(bool isDark, Random rng)
            => isDark ? 8f + rng.NextSingle() * 12f : 80f + rng.NextSingle() * 15f;

        private static float AccentSaturation(Random rng)
            => 55f + rng.NextSingle() * 35f;

        private static float AccentLightness(Random rng)
            => 40f + rng.NextSingle() * 25f;

        private static float NoiseLightness(bool isLight)
            => isLight ? 85f : 15f;

        private static float NormHue(float h)
            => ((h % 360f) + 360f) % 360f;

        private static SKColor TitleColor(float hue, bool isDark)
            => SKColor.FromHsl(NormHue(hue), 0f, isDark ? 93f : 7f);

        private static SKColor SubtitleColor(float hue, bool isDark)
            => SKColor.FromHsl(NormHue(hue), 20f, isDark ? 90f : 12f);

        public static SKColor ShiftHue(SKColor color, float degrees)
        {
            color.ToHsl(out float h, out float s, out float l);
            h = ((h + degrees) % 360f + 360f) % 360f;
            return SKColor.FromHsl(h, s, l);
        }
    }
}
