using SkiaSharp;

namespace backend.Services.AlbumCoverGeneration
{
    public enum TextLayout { BothBottom, BothTop, TitleTopSubtitleBottom }

    public static class TextTitle
    {
        private static readonly SKFontStyle[] TitleStyles =
        [
            new(SKFontStyleWeight.Black,     SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            new(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            new(SKFontStyleWeight.Bold,      SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            new(SKFontStyleWeight.Bold,      SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
        ];

        private static readonly SKFontStyle[] SubtitleStyles =
        [
            new(SKFontStyleWeight.Light,  SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            new(SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            new(SKFontStyleWeight.Medium, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            new(SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
        ];

        public static void Draw(SKCanvas canvas, int size, ColorPalette palette,
            string title, string artist, TextLayout layout, Random rng, TitleConfig? config = null)
        {
            config ??= new TitleConfig();
            if (IsEmpty(title) && IsEmpty(artist)) return;

            string displayTitle = FormatTitle(title, rng);
            using var titleTypeface = CreateTypeface(config.FontFamily, TitleStyles, rng);
            using var subtitleTypeface = CreateTypeface(config.FontFamily, SubtitleStyles, rng);

            var titleBlock = MeasureBlock(displayTitle, titleTypeface, config, size, layout, isTitle: true);
            var subtitleBlock = MeasureBlock(artist, subtitleTypeface, config, size, layout, isTitle: false);
            var (titleY, subtitleY) = ComputeYPositions(layout, size, config, titleBlock, subtitleBlock);

            RenderLines(canvas, titleBlock, titleY, palette.TextTitle, size, config);
            RenderLines(canvas, subtitleBlock, subtitleY, palette.TextSubtitle, size, config);
        }

        private static bool IsEmpty(string text)
            => string.IsNullOrWhiteSpace(text);

        private static string FormatTitle(string title, Random rng)
            => rng.Next(2) == 0 ? title.ToUpperInvariant() : CapitalizeFirstLetters(title);

        private static string CapitalizeFirstLetters(string text)
            => string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(CapitalizeWord));

        private static string CapitalizeWord(string word)
            => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();

        private static SKTypeface CreateTypeface(string family, SKFontStyle[] styles, Random rng)
            => SKTypeface.FromFamilyName(family, PickStyle(styles, rng));

        private static SKFontStyle PickStyle(SKFontStyle[] styles, Random rng)
            => styles[rng.Next(styles.Length)];

        private record TextBlock(List<string> Lines, float LineHeight, float FontSize, SKTypeface Typeface);

        private static TextBlock MeasureBlock(string text, SKTypeface typeface, TitleConfig config, int size, TextLayout layout, bool isTitle)
        {
            var bounds = FitBounds(size, layout, config, isTitle);
            float fontSize = ResolveFontSizeFitting(text, typeface, config.MinFontSize, MaxSize(config, isTitle), bounds, singleLineOnly: isTitle);
            var (lines, lineH) = WrapText(text, typeface, fontSize, bounds.Width);
            return new TextBlock(lines, lineH, fontSize, typeface);
        }

        private static float MaxSize(TitleConfig config, bool isTitle)
            => isTitle ? config.MaxTitleFontSize : config.MaxSubtitleFontSize;

        private static SKRect FitBounds(int s, TextLayout l, TitleConfig c, bool isTitle)
            => SKRect.Create(c.Margin, 0, ContentWidth(s, c), HeightFactor(l, c, isTitle) * s);

        private static float ContentWidth(int s, TitleConfig c)
            => s - 2 * c.Margin;

        private static float HeightFactor(TextLayout l, TitleConfig c, bool isTitle)
            => (l, isTitle) switch
            {
                (TextLayout.BothTop, true) => c.TitleHeightBothTopFactor,
                (TextLayout.TitleTopSubtitleBottom, true) => c.TitleHeightSplitFactor,
                (_, true) => c.TitleHeightDefaultFactor,
                (TextLayout.BothTop, false) => c.SubtitleHeightBothTopFactor,
                (TextLayout.TitleTopSubtitleBottom, false) => c.SubtitleHeightSplitFactor,
                _ => c.SubtitleHeightDefaultFactor
            };

        private static (float TitleY, float SubtitleY) ComputeYPositions(
            TextLayout layout, int size, TitleConfig config, TextBlock title, TextBlock subtitle)
        {
            float titleH = BlockHeight(title, config);
            float subtitleH = BlockHeight(subtitle, config);
            return layout switch
            {
                TextLayout.TitleTopSubtitleBottom => SplitPositions(size, config, title, subtitle, subtitleH),
                TextLayout.BothTop => TopPositions(config, title, titleH, subtitle),
                _ => BottomPositions(size, config, titleH, subtitleH, title, subtitle)
            };
        }

        private static float BlockHeight(TextBlock b, TitleConfig c)
            => b.Lines.Count > 0 ? b.Lines.Count * b.LineHeight * c.LineSpacing : 0f;

        private static float GapIfPresent(TextBlock subtitle, TitleConfig c)
            => subtitle.Lines.Count > 0 ? c.Gap : 0f;

        private static (float, float) SplitPositions(int size, TitleConfig c, TextBlock title, TextBlock subtitle, float subtitleH)
            => (c.Margin + title.LineHeight, size - c.Margin - subtitleH + subtitle.LineHeight);

        private static (float, float) TopPositions(TitleConfig c, TextBlock title, float titleH, TextBlock subtitle)
            => (c.Margin + title.LineHeight, c.Margin + titleH + GapIfPresent(subtitle, c) + subtitle.LineHeight);

        private static (float, float) BottomPositions(int size, TitleConfig c, float titleH, float subtitleH, TextBlock title, TextBlock subtitle)
        {
            float total = titleH + GapIfPresent(subtitle, c) + subtitleH;
            float top = size - c.Margin - total;
            return (top + title.LineHeight, top + titleH + GapIfPresent(subtitle, c) + subtitle.LineHeight);
        }

        private static void RenderLines(SKCanvas canvas, TextBlock block, float startY, SKColor color, int size, TitleConfig config)
        {
            if (block.Lines.Count == 0) return;
            using var font = MakeFont(block.Typeface, block.FontSize);
            using var paint = MakePaint(color);
            DrawAllLines(canvas, block.Lines, startY, font, paint, size, config);
        }

        private static void DrawAllLines(SKCanvas canvas, List<string> lines, float startY, SKFont font, SKPaint paint, int size, TitleConfig config)
        {
            for (int i = 0; i < lines.Count; i++)
                DrawLine(canvas, lines[i], CenterX(font, lines[i], size, config), LineY(startY, i, font, config), font, paint);
        }

        private static float CenterX(SKFont font, string line, int size, TitleConfig config)
            => config.Margin + (ContentWidth(size, config) - font.MeasureText(line)) / 2f;

        private static float LineY(float startY, int index, SKFont font, TitleConfig config)
            => startY + index * font.Spacing * config.LineSpacing;

        private static void DrawLine(SKCanvas canvas, string text, float x, float y, SKFont font, SKPaint paint)
            => canvas.DrawText(text, x, y, font, paint);

        private static float ResolveFontSizeFitting(string text, SKTypeface tf, float lo, float hi, SKRect bounds, bool singleLineOnly)
        {
            float best = lo;
            while (hi - lo > 0.5f)
                best = TryMid(text, tf, ref lo, ref hi, bounds, best, singleLineOnly);
            return best;
        }

        private static float TryMid(string text, SKTypeface tf, ref float lo, ref float hi, SKRect bounds, float best, bool singleLineOnly)
        {
            float mid = (lo + hi) / 2f;
            if (FitsInBounds(text, tf, mid, bounds, singleLineOnly)) { best = mid; lo = mid + 0.5f; }
            else hi = mid - 0.5f;
            return best;
        }

        private static bool FitsInBounds(string text, SKTypeface tf, float size, SKRect bounds, bool singleLineOnly)
        {
            var (lines, lineH) = WrapText(text, tf, size, bounds.Width);
            if (singleLineOnly && lines.Count > 1) return false;
            return lines.Count > 0 && lines.Count * lineH * 1.1f <= bounds.Height;
        }

        private static (List<string> Lines, float LineHeight) WrapText(string text, SKTypeface tf, float size, float maxWidth)
        {
            using var font = MakeFont(tf, size);
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Length == 0 ? ([], font.Spacing) : (BuildLines(font, words, maxWidth), font.Spacing);
        }

        private static List<string> BuildLines(SKFont font, string[] words, float maxWidth)
        {
            var lines = new List<string>();
            string current = words[0];
            for (int i = 1; i < words.Length; i++)
                current = AppendOrBreak(font, lines, current, words[i], maxWidth);
            lines.Add(current);
            return lines;
        }

        private static string AppendOrBreak(SKFont font, List<string> lines, string cur, string word, float max)
        {
            string candidate = cur + " " + word;
            if (font.MeasureText(candidate) <= max) return candidate;
            lines.Add(cur);
            return word;
        }

        private static SKFont MakeFont(SKTypeface tf, float size)
            => new(tf, size) { Edging = SKFontEdging.SubpixelAntialias, Subpixel = true };

        private static SKPaint MakePaint(SKColor color)
            => new() { IsAntialias = true, Color = color };
    }
}