using System.Net.Http;
using System.Text.Json;
using SkiaSharp;
using Svg.Skia;

namespace backend.Services.AlbumCoverGeneration
{
    public enum DiceBearVerticalPosition
    {
        Top,
        Center,
        Bottom
    }

    public static class DiceBear
    {
        private static readonly HttpClient _http = new()
        {
            BaseAddress = new Uri("https://api.dicebear.com/10.x/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static readonly Lazy<List<string>> _styles = new(() => LoadStyles());

        private static List<string> LoadStyles()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "GenerationConfig.json");
            string json = File.ReadAllText(configPath);
            using var doc = JsonDocument.Parse(json);
            var stylesArray = doc.RootElement
                .GetProperty("DiceBear")
                .GetProperty("Styles");

            var styles = new List<string>();
            foreach (var item in stylesArray.EnumerateArray())
            {
                styles.Add(item.GetString()!);
            }

            if (styles.Count == 0)
                throw new InvalidOperationException("GenerationConfig.json must contain at least one DiceBear style.");

            return styles;
        }

        public static async Task Draw(
            SKCanvas canvas, int canvasSize, Random rng,
            float scale = 0.8f, DiceBearVerticalPosition position = DiceBearVerticalPosition.Center)
        {
            string style = PickStyle(rng);
            string seedValue = rng.Next(0, 1_000_000).ToString();
            string svgMarkup = await GetSvgAsync(style, seedValue);
            DrawSvgOnCanvas(canvas, canvasSize, svgMarkup, scale, position);
        }

        private static string PickStyle(Random rng)
        {
            var styles = _styles.Value;
            return styles[rng.Next(styles.Count)];
        }

        public static async Task<string> GetSvgAsync(string style, string seedValue)
        {
            try
            {
                string requestUri = $"{Uri.EscapeDataString(style)}/svg?seed={Uri.EscapeDataString(seedValue)}";
                HttpResponseMessage response = await _http.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get SVG from DiceBear API for style '{style}' and seed '{seedValue}'.", ex);
            }

        }

        private static void DrawSvgOnCanvas(
            SKCanvas canvas, int canvasSize, string svgMarkup,
            float iconScale, DiceBearVerticalPosition position)
        {
            using var svg = new SKSvg();
            svg.FromSvg(svgMarkup);

            if (svg.Picture == null)
                return;

            SKRect svgBounds = svg.Picture.CullRect;
            if (svgBounds.Width <= 0 || svgBounds.Height <= 0)
                return;

            float scale = Math.Max(canvasSize / svgBounds.Width, canvasSize / svgBounds.Height) * iconScale;
            float scaledW = svgBounds.Width * scale;
            float scaledH = svgBounds.Height * scale;
            float offsetX = (canvasSize - scaledW) / 2f;

            float padding = canvasSize * 0.1f;

            float offsetY = position switch
            {
                DiceBearVerticalPosition.Top => padding,
                DiceBearVerticalPosition.Bottom => canvasSize - scaledH - padding,
                _ => (canvasSize - scaledH) / 2f
            };

            canvas.Save();
            canvas.Translate(offsetX, offsetY);
            canvas.Scale(scale);
            canvas.Translate(-svgBounds.Left, -svgBounds.Top);
            canvas.DrawPicture(svg.Picture);
            canvas.Restore();
        }
    }
}
