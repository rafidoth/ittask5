namespace backend.Services.AlbumCoverGeneration
{
    public class TitleConfig
    {
        public float Margin { get; set; } = 60f;
        public float Gap { get; set; } = 8f;

        public float MinFontSize { get; set; } = 40f;
        public float MaxTitleFontSize { get; set; } = 120f;
        public float MaxSubtitleFontSize { get; set; } = 48f;

        public float LineSpacing { get; set; } = 1.1f;
        public string FontFamily { get; set; } = "sans-serif";

        public float TitleHeightBothTopFactor { get; set; } = 0.3f;
        public float TitleHeightSplitFactor { get; set; } = 0.35f;
        public float TitleHeightDefaultFactor { get; set; } = 0.3f;

        public float SubtitleHeightBothTopFactor { get; set; } = 0.15f;
        public float SubtitleHeightSplitFactor { get; set; } = 0.15f;
        public float SubtitleHeightDefaultFactor { get; set; } = 0.12f;
    }
}
