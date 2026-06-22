using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{
    public class Overlay
    {
        public static void Apply(SKCanvas canvas, int canvasSize, Random rng, ColorPalette palette)
        {
            DarkScreen.Apply(canvas, canvasSize, rng);
            DustScratches.Apply(canvas, canvasSize, rng, palette);
            NoisyOverlay.Apply(canvas, canvasSize, palette, rng);
        }

    }

    static class NoisyOverlay
    {
        public static void Apply(SKCanvas canvas, int size, ColorPalette palette, Random rng)
        {
            int speckCount = 1200 + rng.Next(1800);
            using var paint = new SKPaint { IsAntialias = true, BlendMode = SKBlendMode.SrcOver };
            for (int i = 0; i < speckCount; i++)
                DrawSpeck(canvas, paint, size, size * 0.008f, palette, rng);
        }

        private static void DrawSpeck(SKCanvas canvas, SKPaint paint, int size, float maxR, ColorPalette palette, Random rng)
        {
            paint.Color = PickColor(palette, rng).WithAlpha((byte)(40 + rng.Next(100)));
            canvas.DrawCircle(rng.NextSingle() * size, rng.NextSingle() * size, 0.5f + rng.NextSingle() * maxR, paint);
        }

        private static SKColor PickColor(ColorPalette p, Random rng) =>
            rng.Next(3) == 0 ? p.NoiseDark : p.NoiseLight;
    }

    static class DustScratches
    {
        public static void Apply(SKCanvas canvas, int size, Random rng, ColorPalette palette)
        {
            using var paint = new SKPaint { IsAntialias = true, BlendMode = SKBlendMode.SrcOver };
            DrawAllSpecks(canvas, paint, size, palette, rng);
            DrawAllScratches(canvas, paint, size, palette, rng);
            DrawAllSmudges(canvas, paint, size, palette, rng);
            DrawAllGrainClusters(canvas, paint, size, palette, rng);
            DrawAllMicroScratches(canvas, paint, size, palette, rng);
        }

        private static void DrawAllSpecks(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            int count = 600 + rng.Next(800);
            for (int i = 0; i < count; i++)
                DrawSpeck(canvas, paint, size, palette, rng);
        }

        private static void DrawSpeck(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            float x = rng.NextSingle() * size;
            float y = rng.NextSingle() * size;
            DrawDarkSpeck(canvas, paint, x, y, 0.3f + rng.NextSingle() * 2.0f, palette, rng);
            if (rng.Next(3) == 0) DrawLightSpeck(canvas, paint, x, y, palette, rng);
        }

        private static void DrawDarkSpeck(SKCanvas canvas, SKPaint paint, float x, float y, float r, ColorPalette palette, Random rng)
        {
            paint.Color = palette.NoiseDark.WithAlpha((byte)(30 + rng.Next(90)));
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawCircle(x, y, r, paint);
        }

        private static void DrawLightSpeck(SKCanvas canvas, SKPaint paint, float x, float y, ColorPalette palette, Random rng)
        {
            paint.Color = palette.NoiseLight.WithAlpha((byte)(20 + rng.Next(60)));
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawCircle(x + rng.NextSingle() * 4 - 2f, y + rng.NextSingle() * 4 - 2f, 0.2f + rng.NextSingle() * 1.2f, paint);
        }

        private static void DrawAllScratches(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            int count = 60 + rng.Next(30);
            for (int i = 0; i < count; i++)
                DrawScratch(canvas, paint, size, palette, rng);
        }

        private static void DrawScratch(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            float angle = rng.NextSingle() * 2 * MathF.PI;
            float length = size * (0.04f + rng.NextSingle() * 0.22f);
            var (start, mid, end) = ScratchPoints(rng, size, angle, length);
            DrawScratchPath(canvas, paint, start, mid, end, palette, rng);
            if (rng.Next(2) == 0) DrawParallelScratch(canvas, paint, start, mid, end, angle, palette, rng);
        }

        private static (SKPoint s, SKPoint m, SKPoint e) ScratchPoints(Random rng, int size, float angle, float len)
        {
            float x = rng.NextSingle() * size, y = rng.NextSingle() * size;
            float ex = x + len * MathF.Cos(angle), ey = y + len * MathF.Sin(angle);
            float mx = (x + ex) / 2 + (rng.NextSingle() - 0.5f) * len * 0.25f;
            float my = (y + ey) / 2 + (rng.NextSingle() - 0.5f) * len * 0.25f;
            return (new SKPoint(x, y), new SKPoint(mx, my), new SKPoint(ex, ey));
        }

        private static void DrawScratchPath(SKCanvas canvas, SKPaint paint, SKPoint s, SKPoint m, SKPoint e, ColorPalette palette, Random rng)
        {
            ConfigureScratchPaint(paint, 0.4f + rng.NextSingle() * 1.8f, palette.NoiseDark.WithAlpha((byte)(40 + rng.Next(100))));
            using var path = BuildQuadPath(s, m, e);
            canvas.DrawPath(path, paint);
        }

        private static void DrawParallelScratch(SKCanvas canvas, SKPaint paint, SKPoint s, SKPoint m, SKPoint e, float angle, ColorPalette palette, Random rng)
        {
            float off = 1 + rng.NextSingle() * 3, pa = angle + MathF.PI / 2;
            var ov = new SKPoint(off * MathF.Cos(pa), off * MathF.Sin(pa));
            ConfigureScratchPaint(paint, 0.2f + rng.NextSingle() * 0.6f, palette.NoiseLight.WithAlpha((byte)(20 + rng.Next(50))));
            using var path = BuildQuadPath(Offset(s, ov), Offset(m, ov), Offset(e, ov));
            canvas.DrawPath(path, paint);
        }

        private static SKPoint Offset(SKPoint p, SKPoint o) =>
            new(p.X + o.X, p.Y + o.Y);

        private static SKPath BuildQuadPath(SKPoint s, SKPoint m, SKPoint e)
        {
            var path = new SKPath();
            path.MoveTo(s);
            path.QuadTo(m, e);
            return path;
        }

        private static void ConfigureScratchPaint(SKPaint paint, float width, SKColor color)
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.StrokeWidth = width;
            paint.Color = color;
        }

        private static void DrawAllSmudges(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            int count = 8 + rng.Next(16);
            for (int i = 0; i < count; i++)
                DrawSmudge(canvas, paint, size, palette, rng);
        }

        private static void DrawSmudge(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            float cx = rng.NextSingle() * size, cy = rng.NextSingle() * size;
            float radius = size * (0.01f + rng.NextSingle() * 0.03f);
            paint.Color = SmudgeColor(palette, rng).WithAlpha((byte)(10 + rng.Next(30)));
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawOval(cx, cy, radius, radius * (0.5f + rng.NextSingle()), paint);
        }

        private static SKColor SmudgeColor(ColorPalette p, Random rng) =>
            rng.Next(2) == 0 ? p.NoiseDark : p.NoiseLight;

        private static void DrawAllGrainClusters(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            int count = 20 + rng.Next(40);
            for (int i = 0; i < count; i++)
                DrawGrainCluster(canvas, paint, size, palette, rng);
        }

        private static void DrawGrainCluster(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            float cx = rng.NextSingle() * size, cy = rng.NextSingle() * size;
            int grains = 5 + rng.Next(15);
            for (int g = 0; g < grains; g++)
                DrawGrain(canvas, paint, cx, cy, size * 0.015f, palette, rng);
        }

        private static void DrawGrain(SKCanvas canvas, SKPaint paint, float cx, float cy, float spread, ColorPalette palette, Random rng)
        {
            paint.Color = palette.NoiseDark.WithAlpha((byte)(20 + rng.Next(60)));
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawCircle(cx + (rng.NextSingle() - 0.5f) * spread * 2, cy + (rng.NextSingle() - 0.5f) * spread * 2, 0.2f + rng.NextSingle() * 0.8f, paint);
        }

        private static void DrawAllMicroScratches(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            int count = 30 + rng.Next(50);
            for (int i = 0; i < count; i++)
                DrawMicroScratch(canvas, paint, size, palette, rng);
        }

        private static void DrawMicroScratch(SKCanvas canvas, SKPaint paint, int size, ColorPalette palette, Random rng)
        {
            float x = rng.NextSingle() * size, y = rng.NextSingle() * size;
            float len = size * (0.005f + rng.NextSingle() * 0.02f);
            float angle = rng.NextSingle() * MathF.PI * 2;
            ConfigureScratchPaint(paint, 0.2f + rng.NextSingle() * 0.5f, palette.NoiseDark.WithAlpha((byte)(25 + rng.Next(45))));
            canvas.DrawLine(x, y, x + len * MathF.Cos(angle), y + len * MathF.Sin(angle), paint);
        }
    }


    public static class DarkScreen
    {
        public static void Apply(SKCanvas canvas, int size, Random rng)
        {
            // Dark vignette - darker edges
            using var paint = new SKPaint
            {
                IsAntialias = true,
                BlendMode = SKBlendMode.Multiply,  // Darkens underlying colors
                Shader = CreateRadialGradient(size, rng)
            };

            // Draw full-screen rect with gradient
            canvas.DrawRect(0, 0, size, size, paint);

            // Optional: additional dark noise
            if (rng.Next(2) == 0)
                ApplyDarkNoise(canvas, size, rng);
        }

        private static SKShader CreateRadialGradient(int size, Random rng)
        {
            float centerX = size * (0.3f + rng.NextSingle() * 0.4f);
            float centerY = size * (0.3f + rng.NextSingle() * 0.4f);
            float radius = size * 0.7f;

            var colors = new SKColor[]
            {
                SKColors.Transparent,           // Center - fully transparent
                SKColors.Transparent,           // Mid - still transparent
                new SKColor(0, 0, 0, 100),      // 40% opacity dark
                new SKColor(0, 0, 0, 180),      // 70% opacity dark at edges
            };

            float[] positions = new float[]
            {
                0.0f,
                0.3f + rng.NextSingle() * 0.2f,
                0.6f + rng.NextSingle() * 0.2f,
                1.0f
            };

            return SKShader.CreateRadialGradient(
                new SKPoint(centerX, centerY),
                radius,
                colors,
                positions,
                SKShaderTileMode.Clamp
            );
        }

        private static void ApplyDarkNoise(SKCanvas canvas, int size, Random rng)
        {
            int dotCount = 100 + rng.Next(200);
            using var paint = new SKPaint
            {
                IsAntialias = true,
                BlendMode = SKBlendMode.Darken
            };

            for (int i = 0; i < dotCount; i++)
            {
                float x = rng.NextSingle() * size;
                float y = rng.NextSingle() * size;
                float radius = 0.5f + rng.NextSingle() * 2f;

                paint.Color = new SKColor(0, 0, 0, (byte)(30 + rng.Next(70)));
                canvas.DrawCircle(x, y, radius, paint);
            }
        }
    }
}