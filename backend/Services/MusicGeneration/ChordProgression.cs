namespace backend.Services.MusicGeneration;

public static class ChordProgression
{
    private static readonly (int degree, ChordQuality quality)[][] MajorTemplates =
    [
        [(0, ChordQuality.Major), (4, ChordQuality.Major), (5, ChordQuality.Minor), (3, ChordQuality.Major)],
        [(0, ChordQuality.Major), (3, ChordQuality.Major), (4, ChordQuality.Major), (3, ChordQuality.Major)],
        [(0, ChordQuality.Major), (5, ChordQuality.Minor), (3, ChordQuality.Major), (4, ChordQuality.Major)],
        [(1, ChordQuality.Minor), (4, ChordQuality.Dominant7), (0, ChordQuality.Major7), (0, ChordQuality.Major7)],
    ];

    private static readonly (int degree, ChordQuality quality)[][] MinorTemplates =
    [
        [(0, ChordQuality.Minor), (3, ChordQuality.Major), (6, ChordQuality.Major), (2, ChordQuality.Major)],
        [(0, ChordQuality.Minor), (5, ChordQuality.Major), (3, ChordQuality.Major), (4, ChordQuality.Major)],
        [(0, ChordQuality.Minor), (6, ChordQuality.Major), (5, ChordQuality.Major), (4, ChordQuality.Major)],
        [(0, ChordQuality.Minor), (3, ChordQuality.Major), (5, ChordQuality.Major), (6, ChordQuality.Major)],
    ];

    public static int[][] Generate(int rootMidi, ScaleType scaleType, DeterministicRandom rng)
    {
        var templates = SelectTemplates(scaleType);
        var template = rng.Choose(templates);
        return BuildFromTemplate(rootMidi, scaleType, template);
    }

    private static (int, ChordQuality)[][] SelectTemplates(ScaleType type) =>
        IsMinorScale(type) ? MinorTemplates : MajorTemplates;

    private static bool IsMinorScale(ScaleType type) =>
        type is ScaleType.NaturalMinor or ScaleType.HarmonicMinor or ScaleType.Dorian or ScaleType.MinorPentatonic or ScaleType.Blues;

    private static int[][] BuildFromTemplate(int rootMidi, ScaleType scaleType, (int degree, ChordQuality quality)[] template)
    {
        var scaleIntervals = Scale.GetIntervals(scaleType);
        return template.Select(t => BuildChordFromDegree(rootMidi, scaleIntervals, t.degree, t.quality)).ToArray();
    }

    private static int[] BuildChordFromDegree(int rootMidi, int[] scaleIntervals, int degree, ChordQuality quality)
    {
        int chordRoot = rootMidi + GetScaleInterval(scaleIntervals, degree);
        return Chord.Build(chordRoot, quality);
    }

    private static int GetScaleInterval(int[] intervals, int degree) =>
        intervals[degree % intervals.Length];

    public static int GetProgressionLength(int[][] progression) =>
        progression.Length;
}
