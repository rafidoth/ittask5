namespace backend.Services.MusicGeneration;

public static class Music
{
    public static byte[] Generate(int seed, int songId)
    {
        var rng = CreateRng(seed, songId);
        var structure = BuildSongStructure(rng);
        return RenderAndExport(structure, rng);
    }

    private static DeterministicRandom CreateRng(int seed, int songId)
    {
        unchecked { return new((seed * (int)2654435761u) ^ (songId * 40503)); }
    }

    private static SongStructure BuildSongStructure(DeterministicRandom rng)
    {
        var style = ChooseStyle(rng);
        var bpm = ChooseTempo(style, rng);
        var scaleType = ChooseScale(style, rng);
        var rootNote = ChooseRootNote(rng);
        return AssembleStructure(style, bpm, scaleType, rootNote, rng);
    }

    private static SongStyle ChooseStyle(DeterministicRandom rng)
    {
        var styleType = rng.Choose<SongStyleType>([SongStyleType.PopRock, SongStyleType.EdmSynthwave, SongStyleType.AmbientLofi]);
        return SongStyle.Create(styleType, rng);
    }

    private static double ChooseTempo(SongStyle style, DeterministicRandom rng) =>
        rng.NextInt(style.MinTempo, style.MaxTempo + 1);

    private static ScaleType ChooseScale(SongStyle style, DeterministicRandom rng) =>
        rng.Choose(style.PreferredScales);

    private static int ChooseRootNote(DeterministicRandom rng) =>
        Pitch.C3 + rng.NextInt(0, 7);

    private static SongStructure AssembleStructure(SongStyle style, double bpm, ScaleType scaleType, int rootNote, DeterministicRandom rng)
    {
        var progression = GenerateChordProgression(rootNote, scaleType, rng);
        var scaleNotes = Scale.GetScaleNotes(rootNote, scaleType, 3);
        return PopulateStructure(style, bpm, rootNote, scaleType, progression, scaleNotes, rng);
    }

    private static int[][] GenerateChordProgression(int rootNote, ScaleType scaleType, DeterministicRandom rng) =>
        ChordProgression.Generate(rootNote, scaleType, rng);

    private static SongStructure PopulateStructure(SongStyle style, double bpm, int rootNote, ScaleType scaleType, int[][] progression, int[] scaleNotes, DeterministicRandom rng) => new()
    {
        Style = style,
        Bpm = bpm,
        RootNote = rootNote,
        ScaleType = scaleType,
        ChordProgression = progression,
        TotalBars = style.SongBars,
        MelodyNotes = GenerateMelody(scaleNotes, progression, style, rng),
        BassNotes = GenerateBass(progression, style, rng),
        ArpNotes = GenerateArp(progression, style, rng),
        PadNotes = GeneratePad(progression, style),
        DrumHits = GenerateDrums(style, rng)
    };

    private static List<Note> GenerateMelody(int[] scaleNotes, int[][] progression, SongStyle style, DeterministicRandom rng) =>
        MelodyGenerator.Generate(scaleNotes, progression, style.SongBars, style.NoteDensity, style.RestProbability, rng);

    private static List<Note> GenerateBass(int[][] progression, SongStyle style, DeterministicRandom rng) =>
        MelodyGenerator.GenerateBassLine(progression, style.SongBars, rng);

    private static List<Note> GenerateArp(int[][] progression, SongStyle style, DeterministicRandom rng) =>
        MelodyGenerator.GenerateArpeggio(progression, style.SongBars, style.NoteDensity, rng);

    private static List<Note> GeneratePad(int[][] progression, SongStyle style) =>
        MelodyGenerator.GeneratePadChords(progression, style.SongBars);

    private static List<DrumHit> GenerateDrums(SongStyle style, DeterministicRandom rng) =>
        DrumPatterns.Generate(style.PreferredDrumPattern, style.SongBars, rng);

    private static byte[] RenderAndExport(SongStructure structure, DeterministicRandom rng)
    {
        var buffer = AudioRenderer.RenderSong(structure);
        ApplyPostProcessing(buffer, structure.Style);
        return WavExporter.Export(buffer.GetSamples());
    }

    private static void ApplyPostProcessing(AudioBuffer buffer, SongStyle style)
    {
        var samples = buffer.GetSamples();
        ApplyEffects(samples, style);
        FinalizeAudio(samples);
    }

    private static void ApplyEffects(float[] samples, SongStyle style)
    {
        int delaySamples = CalculateDelaySamples(style);
        Effects.ApplyDelay(samples, delaySamples, style.DelayFeedback, style.DelayMix);
        Effects.ApplyReverb(samples, (float)style.ReverbRoomSize, style.ReverbDampening, style.ReverbMix);
    }

    private static int CalculateDelaySamples(SongStyle style) =>
        (int)(AudioBuffer.SampleRate * 0.375);

    private static void FinalizeAudio(float[] samples)
    {
        Effects.Normalize(samples);
        Effects.ApplyFadeOut(samples, CalculateFadeSamples());
    }

    private static int CalculateFadeSamples() =>
        AudioBuffer.SampleRate * 3;
}
