namespace backend.Services.MusicGeneration;

public static class MelodyGenerator
{
    public static List<Note> Generate(int[] scaleNotes, int[][] chordProgression, int bars, double noteDensity, double restProbability, DeterministicRandom rng)
    {
        var melody = new List<Note>();
        int currentNoteIndex = FindMiddleIndex(scaleNotes);
        for (int bar = 0; bar < bars; bar++)
            currentNoteIndex = GenerateBar(melody, scaleNotes, chordProgression, bar, currentNoteIndex, noteDensity, restProbability, rng);
        return melody;
    }

    private static int FindMiddleIndex(int[] scaleNotes) =>
        scaleNotes.Length / 2;

    private static int GenerateBar(List<Note> melody, int[] scaleNotes, int[][] progression, int bar, int noteIndex, double density, double restProb, DeterministicRandom rng)
    {
        int[] currentChord = GetChordForBar(progression, bar);
        double beatPosition = 0;
        while (beatPosition < 4.0)
            beatPosition = GenerateBeat(melody, scaleNotes, currentChord, ref noteIndex, beatPosition, density, restProb, rng);
        return noteIndex;
    }

    private static int[] GetChordForBar(int[][] progression, int bar) =>
        progression[bar % progression.Length];

    private static double GenerateBeat(List<Note> melody, int[] scaleNotes, int[] chord, ref int noteIndex, double beatPosition, double density, double restProb, DeterministicRandom rng)
    {
        double duration = ChooseNoteDuration(rng, density);
        Note note = CreateMelodyNote(scaleNotes, chord, ref noteIndex, duration, restProb, rng);
        melody.Add(note);
        return beatPosition + duration;
    }

    private static double ChooseNoteDuration(DeterministicRandom rng, double density)
    {
        double[] durations = GetDurationsForDensity(density);
        return rng.Choose(durations);
    }

    private static double[] GetDurationsForDensity(double density) => density switch
    {
        > 0.8 => [Duration.Sixteenth, Duration.Eighth, Duration.Eighth, Duration.Quarter],
        > 0.6 => [Duration.Eighth, Duration.Quarter, Duration.Quarter, Duration.DottedEighth],
        > 0.4 => [Duration.Quarter, Duration.Quarter, Duration.Half, Duration.DottedQuarter],
        _ => [Duration.Quarter, Duration.Half, Duration.Half, Duration.Whole]
    };

    private static Note CreateMelodyNote(int[] scaleNotes, int[] chord, ref int noteIndex, double duration, double restProb, DeterministicRandom rng)
    {
        if (ShouldRest(rng, restProb))
            return Note.CreateRest(duration);
        return GeneratePitchedNote(scaleNotes, chord, ref noteIndex, duration, rng);
    }

    private static bool ShouldRest(DeterministicRandom rng, double restProbability) =>
        rng.NextDouble() < restProbability;

    private static Note GeneratePitchedNote(int[] scaleNotes, int[] chord, ref int noteIndex, double duration, DeterministicRandom rng)
    {
        noteIndex = ChooseNextNoteIndex(scaleNotes, chord, noteIndex, rng);
        int midiNote = scaleNotes[noteIndex];
        return Note.CreateWithRandomVelocity(midiNote, duration, rng);
    }

    private static int ChooseNextNoteIndex(int[] scaleNotes, int[] chord, int currentIndex, DeterministicRandom rng)
    {
        int strategy = rng.NextInt(0, 4);
        return ApplyMelodicStrategy(scaleNotes, chord, currentIndex, strategy, rng);
    }

    private static int ApplyMelodicStrategy(int[] scaleNotes, int[] chord, int currentIndex, int strategy, DeterministicRandom rng) => strategy switch
    {
        0 => StepwiseMotion(scaleNotes, currentIndex, rng),
        1 => LeapMotion(scaleNotes, currentIndex, rng),
        2 => ArpeggioMotion(scaleNotes, chord, rng),
        _ => RepeatOrNeighbor(scaleNotes, currentIndex, rng)
    };

    private static int StepwiseMotion(int[] scaleNotes, int currentIndex, DeterministicRandom rng)
    {
        int step = rng.NextBool() ? 1 : -1;
        return ClampIndex(currentIndex + step, scaleNotes.Length);
    }

    private static int LeapMotion(int[] scaleNotes, int currentIndex, DeterministicRandom rng)
    {
        int leap = rng.NextInt(2, 5) * (rng.NextBool() ? 1 : -1);
        return ClampIndex(currentIndex + leap, scaleNotes.Length);
    }

    private static int ArpeggioMotion(int[] scaleNotes, int[] chord, DeterministicRandom rng)
    {
        int chordNote = rng.Choose(chord);
        return FindClosestIndex(scaleNotes, chordNote);
    }

    private static int RepeatOrNeighbor(int[] scaleNotes, int currentIndex, DeterministicRandom rng)
    {
        int offset = rng.NextInt(-1, 2);
        return ClampIndex(currentIndex + offset, scaleNotes.Length);
    }

    private static int ClampIndex(int index, int length) =>
        Math.Clamp(index, 0, length - 1);

    private static int FindClosestIndex(int[] scaleNotes, int targetMidi)
    {
        int bestIndex = 0;
        for (int i = 1; i < scaleNotes.Length; i++)
            bestIndex = CloserNote(scaleNotes, targetMidi, bestIndex, i);
        return bestIndex;
    }

    private static int CloserNote(int[] notes, int target, int indexA, int indexB) =>
        Math.Abs(notes[indexB] - target) < Math.Abs(notes[indexA] - target) ? indexB : indexA;

    public static List<Note> GenerateArpeggio(int[][] chordProgression, int bars, double noteDensity, DeterministicRandom rng)
    {
        var notes = new List<Note>();
        for (int bar = 0; bar < bars; bar++)
            GenerateArpeggioBar(notes, chordProgression, bar, noteDensity, rng);
        return notes;
    }

    private static void GenerateArpeggioBar(List<Note> notes, int[][] progression, int bar, double density, DeterministicRandom rng)
    {
        int[] chord = GetChordForBar(progression, bar);
        double beatPos = 0;
        while (beatPos < 4.0)
            beatPos = AddArpeggioNote(notes, chord, beatPos, density, rng);
    }

    private static double AddArpeggioNote(List<Note> notes, int[] chord, double beatPos, double density, DeterministicRandom rng)
    {
        double duration = density > 0.7 ? Duration.Sixteenth : Duration.Eighth;
        int chordNote = chord[((int)(beatPos / duration)) % chord.Length];
        notes.Add(Note.CreateWithRandomVelocity(chordNote, duration, rng));
        return beatPos + duration;
    }

    public static List<Note> GenerateBassLine(int[][] chordProgression, int bars, DeterministicRandom rng)
    {
        var notes = new List<Note>();
        for (int bar = 0; bar < bars; bar++)
            GenerateBassBar(notes, chordProgression, bar, rng);
        return notes;
    }

    private static void GenerateBassBar(List<Note> notes, int[][] progression, int bar, DeterministicRandom rng)
    {
        int[] chord = GetChordForBar(progression, bar);
        int bassNote = Pitch.OctaveShift(Chord.GetRoot(chord), -2);
        AddBassPattern(notes, bassNote, rng);
    }

    private static void AddBassPattern(List<Note> notes, int bassNote, DeterministicRandom rng)
    {
        int pattern = rng.NextInt(0, 3);
        var patternNotes = SelectBassPattern(bassNote, pattern);
        notes.AddRange(patternNotes);
    }

    private static List<Note> SelectBassPattern(int root, int pattern) => pattern switch
    {
        0 => [Note.Create(root, Duration.Half, 0.85f), Note.Create(root, Duration.Half, 0.75f)],
        1 => [Note.Create(root, Duration.Quarter, 0.85f), Note.CreateRest(Duration.Quarter), Note.Create(root, Duration.Quarter, 0.8f), Note.Create(root + 7, Duration.Quarter, 0.7f)],
        _ => [Note.Create(root, Duration.Whole, 0.8f)]
    };

    public static List<Note> GeneratePadChords(int[][] chordProgression, int bars)
    {
        var notes = new List<Note>();
        for (int bar = 0; bar < bars; bar++)
            AddPadBar(notes, chordProgression, bar);
        return notes;
    }

    private static void AddPadBar(List<Note> notes, int[][] progression, int bar)
    {
        int[] chord = GetChordForBar(progression, bar);
        foreach (int chordNote in chord)
            notes.Add(Note.Create(chordNote, Duration.Whole, 0.5f));
    }
}
