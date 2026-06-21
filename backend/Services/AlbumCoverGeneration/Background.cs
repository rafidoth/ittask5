using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{

    public enum BackgroundStyle
    {
        Style1,
        Style2,
        Style3,
        Style4,
        Style5,
    }
    public static class Background
    {

        public static void Draw(SKCanvas canvas, int canvasSize, Random rng, ColorPalette palette)
        {
            FillWithBaseColor(canvas, canvasSize, palette, rng);
        }
        private static void FillWithBaseColor(SKCanvas canvas, int size, ColorPalette palette, Random rng)
        {
            canvas.Clear(palette.Background);
            int sectors = 6 + rng.Next(6);
            float centerX = size * (0.3f + rng.NextSingle() * 0.4f);
            float centerY = size * (0.3f + rng.NextSingle() * 0.4f);
            AngularRays.Draw(canvas, size, centerX, centerY, sectors, palette, rng);
        }
    }

    static class AngularRays
    {
        public static void Draw(SKCanvas canvas, int size, float cx, float cy,
       int sectors, ColorPalette palette, Random rng)
        {
            float angleStep = 360f / sectors;
            using var paint = new SKPaint { IsAntialias = true, BlendMode = SKBlendMode.SoftLight };
            for (int i = 0; i < sectors; i++)
                DrawRays(canvas, paint, cx, cy, i, angleStep, size, palette, rng);
        }

        private static void DrawRays(SKCanvas canvas, SKPaint paint, float cx, float cy,
            int i, float angleStep, int size, ColorPalette palette, Random rng)
        {
            paint.Color = (i % 2 == 0 ? palette.BackgroundAlt : palette.Background).WithAlpha((byte)(40 + rng.Next(80)));
            using var path = BuildRayPaths(cx, cy, i * angleStep, angleStep, size, rng);
            canvas.DrawPath(path, paint);
        }

        private static SKPath BuildRayPaths(float cx, float cy, float startAngle, float angleStep, int size, Random rng)
        {
            float r = size * 1.5f;
            float a1 = startAngle * MathF.PI / 180f;
            float a2 = (startAngle + angleStep) * MathF.PI / 180f;
            var path = new SKPath();
            path.MoveTo(cx, cy);
            path.LineTo(cx + r * MathF.Cos(a1), cy + r * MathF.Sin(a1));
            AppendArcPoints(path, cx, cy, r, a1, a2, arcSteps: rng.Next(5, 15));
            path.Close();
            return path;
        }

        private static void AppendArcPoints(SKPath path, float cx, float cy, float r, float a1, float a2, int arcSteps)
        {
            for (int j = 1; j <= arcSteps; j++)
            {
                float a = a1 + (a2 - a1) * j / arcSteps;
                path.LineTo(cx + r * MathF.Cos(a), cy + r * MathF.Sin(a));
            }
        }
    }



}