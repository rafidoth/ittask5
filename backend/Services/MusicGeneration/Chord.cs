namespace backend.Services.MusicGeneration;

public enum ChordQuality
{
    Major,
    Minor,
    Diminished,
    Augmented,
    Dominant7,
    Major7,
    Minor7
}

public static class Chord
{
    public static int[] GetIntervals(ChordQuality quality) => quality switch
    {
        ChordQuality.Major => [0, 4, 7],
        ChordQuality.Minor => [0, 3, 7],
        ChordQuality.Diminished => [0, 3, 6],
        ChordQuality.Augmented => [0, 4, 8],
        ChordQuality.Dominant7 => [0, 4, 7, 10],
        ChordQuality.Major7 => [0, 4, 7, 11],
        ChordQuality.Minor7 => [0, 3, 7, 10],
        _ => [0, 4, 7]
    };

    public static int[] Build(int rootMidi, ChordQuality quality) =>
        GetIntervals(quality).Select(i => rootMidi + i).ToArray();

    public static int[] BuildInversion(int rootMidi, ChordQuality quality, int inversion)
    {
        var notes = Build(rootMidi, quality);
        for (int i = 0; i < inversion && i < notes.Length; i++)
            notes[i] += 12;
        return notes.Order().ToArray();
    }

    public static int GetRoot(int[] chordNotes) =>
        chordNotes[0];

    public static int[] Arpeggiate(int[] chordNotes) =>
        chordNotes;
}
