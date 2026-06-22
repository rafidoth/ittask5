using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{
    public static class Background
    {

        public static void Draw(SKCanvas canvas, int canvasSize, Random rng, ColorPalette palette)
        {
            FillWithBaseColor(canvas, palette, rng);
            AngularRays.Draw(canvas, canvasSize, palette, rng);
            DiagonalPattern.Draw(canvas, canvasSize, palette, rng);
        }
        private static void FillWithBaseColor(SKCanvas canvas, ColorPalette palette, Random rng)
        {
            canvas.Clear(palette.Background);
        }
    }

    static class AngularRays
    {
        public static void Draw(SKCanvas canvas, int size, ColorPalette palette, Random rng)
        {
            int sectors = 6 + rng.Next(6);
            float centerX = size * (0.3f + rng.NextSingle() * 0.4f);
            float centerY = size * (0.3f + rng.NextSingle() * 0.4f);
            float angleStep = 360f / sectors;
            using var paint = new SKPaint { IsAntialias = true, BlendMode = SKBlendMode.SoftLight };
            for (int i = 0; i < sectors; i++)
                DrawRays(canvas, paint, centerX, centerY, i, angleStep, size, palette, rng);
        }

        private static void DrawRays(SKCanvas canvas, SKPaint paint, float cx, float cy,
            int i, float angleStep, int size, ColorPalette palette, Random rng)
        {
            paint.Color = RayColor(palette, i).WithAlpha((byte)(40 + rng.Next(80)));
            using var path = BuildRayPaths(cx, cy, i * angleStep, angleStep, size, rng);
            canvas.DrawPath(path, paint);
        }

        private static SKColor RayColor(ColorPalette palette, int i) => (i % 3) switch
        {
            0 => palette.BackgroundAlt,
            1 => palette.Accent,
            _ => palette.Background
        };

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

    static class DiagonalPattern
    {
        public static void Draw(SKCanvas canvas, int size, ColorPalette palette, Random rng)
        {
            int bandCount = 8 + rng.Next(12) * 2;
            float angle = (25f + rng.NextSingle() * 50f) * (rng.Next(2) == 0 ? -1f : 1f);
            float radians = angle * MathF.PI / 180f;
            float sinA = MathF.Sin(radians), cosA = MathF.Cos(radians);
            float span = Math.Abs(size * cosA) + Math.Abs(size * sinA);
            DrawBandStrips(canvas, palette, rng, size, bandCount, sinA, cosA, span);
        }

        private static void DrawBandStrips(SKCanvas canvas, ColorPalette palette, Random rng,
            int size, int bandCount, float sinA, float cosA, float span)
        {
            using var paint = new SKPaint { IsAntialias = true, BlendMode = SKBlendMode.Multiply };
            float pos = -span * 0.5f;
            for (int i = 0; i < bandCount; i++)
            {
                float width = span / bandCount * (0.5f + rng.NextSingle());
                DrawBandStrip(canvas, paint, palette, rng, size, i, pos + width / 2f, width, sinA, cosA);
                pos += width;
            }
        }

        private static void DrawBandStrip(SKCanvas canvas, SKPaint paint, ColorPalette palette, Random rng,
            int size, int i, float bandMidPos, float width, float sinA, float cosA)
        {
            paint.Color = BandColor(palette, i).WithAlpha((byte)(25 + rng.Next(55)));
            using var path = BuildBandPath(size, bandMidPos, width, sinA, cosA);
            canvas.DrawPath(path, paint);
        }

        private static SKColor BandColor(ColorPalette palette, int i) => (i % 3) switch
        {
            0 => palette.Background,
            1 => palette.Accent,
            _ => palette.BackgroundAlt
        };

        private static SKPath BuildBandPath(int size, float bandMidPos, float width, float sinA, float cosA)
        {
            float halfW = width / 2f, ext = size * 1.5f;
            float bcx = size / 2f + bandMidPos * cosA;
            float bcy = size / 2f + bandMidPos * sinA;
            float px = -sinA, py = cosA, dx = cosA, dy = sinA;
            var path = new SKPath();
            path.MoveTo(bcx - px * ext - dx * halfW, bcy - py * ext - dy * halfW);
            path.LineTo(bcx - px * ext + dx * halfW, bcy - py * ext + dy * halfW);
            path.LineTo(bcx + px * ext + dx * halfW, bcy + py * ext + dy * halfW);
            path.LineTo(bcx + px * ext - dx * halfW, bcy + py * ext - dy * halfW);
            path.Close();
            return path;
        }
    }



}