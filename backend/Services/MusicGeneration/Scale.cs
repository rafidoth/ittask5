namespace backend.Services.MusicGeneration;

public enum ScaleType
{
    Major,
    NaturalMinor,
    HarmonicMinor,
    Dorian,
    Mixolydian,
    Pentatonic,
    MinorPentatonic,
    Blues
}

public static class Scale
{
    public static int[] GetIntervals(ScaleType type) => type switch
    {
        ScaleType.Major => [0, 2, 4, 5, 7, 9, 11],
        ScaleType.NaturalMinor => [0, 2, 3, 5, 7, 8, 10],
        ScaleType.HarmonicMinor => [0, 2, 3, 5, 7, 8, 11],
        ScaleType.Dorian => [0, 2, 3, 5, 7, 9, 10],
        ScaleType.Mixolydian => [0, 2, 4, 5, 7, 9, 10],
        ScaleType.Pentatonic => [0, 2, 4, 7, 9],
        ScaleType.MinorPentatonic => [0, 3, 5, 7, 10],
        ScaleType.Blues => [0, 3, 5, 6, 7, 10],
        _ => [0, 2, 4, 5, 7, 9, 11]
    };

    public static int[] GetScaleNotes(int rootMidi, ScaleType type, int octaveSpan = 2)
    {
        var intervals = GetIntervals(type);
        var notes = new List<int>();
        for (int octave = 0; octave < octaveSpan; octave++)
            AddOctaveNotes(notes, rootMidi, intervals, octave);
        return notes.ToArray();
    }

    private static void AddOctaveNotes(List<int> notes, int root, int[] intervals, int octave)
    {
        foreach (var interval in intervals)
            notes.Add(root + octave * 12 + interval);
    }

    public static int SnapToScale(int midiNote, int[] scaleNotes) =>
        scaleNotes.OrderBy(n => Math.Abs(n - midiNote)).First();

    public static int GetDegree(int[] scaleNotes, int degree)
    {
        int octaveOffset = degree / scaleNotes.Length * 12;
        int index = ((degree % scaleNotes.Length) + scaleNotes.Length) % scaleNotes.Length;
        return scaleNotes[index] + octaveOffset;
    }

    public static int RandomNote(int[] scaleNotes, DeterministicRandom rng) =>
        rng.Choose(scaleNotes);
}
