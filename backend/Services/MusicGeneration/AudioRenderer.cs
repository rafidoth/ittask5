namespace backend.Services.MusicGeneration;

public class SongStructure
{
    public SongStyle Style { get; init; } = null!;
    public double Bpm { get; init; } = 120;
    public int RootNote { get; init; } = Pitch.C3;
    public ScaleType ScaleType { get; init; } = ScaleType.Major;
    public int[][] ChordProgression { get; init; } = [];
    public int TotalBars { get; init; } = 16;
    public List<Note> MelodyNotes { get; init; } = [];
    public List<Note> BassNotes { get; init; } = [];
    public List<Note> ArpNotes { get; init; } = [];
    public List<Note> PadNotes { get; init; } = [];
    public List<DrumHit> DrumHits { get; init; } = [];
}

public static class AudioRenderer
{
    public static AudioBuffer RenderSong(SongStructure structure)
    {
        var buffer = CreateBuffer(structure);
        RenderAllLayers(buffer, structure);
        return buffer;
    }

    private static AudioBuffer CreateBuffer(SongStructure structure)
    {
        double totalBeats = Duration.BarsToBeats(structure.TotalBars);
        double totalSeconds = Duration.BeatsToSeconds(totalBeats, structure.Bpm) + 2.0;
        return new AudioBuffer(totalSeconds);
    }

    private static void RenderAllLayers(AudioBuffer buffer, SongStructure structure)
    {
        RenderBassLayer(buffer, structure);
        RenderPadLayer(buffer, structure);
        RenderArpLayer(buffer, structure);
        RenderMelodyLayer(buffer, structure);
        RenderDrumLayer(buffer, structure);
    }

    private static void RenderBassLayer(AudioBuffer buffer, SongStructure structure) =>
        RenderNoteSequence(buffer, structure.Style.BassInstrument, structure.BassNotes, structure.Bpm);

    private static void RenderPadLayer(AudioBuffer buffer, SongStructure structure) =>
        RenderPadNotes(buffer, structure.Style.PadInstrument, structure.PadNotes, structure.ChordProgression, structure.Bpm, structure.TotalBars);

    private static void RenderArpLayer(AudioBuffer buffer, SongStructure structure) =>
        RenderNoteSequence(buffer, structure.Style.ArpInstrument, structure.ArpNotes, structure.Bpm);

    private static void RenderMelodyLayer(AudioBuffer buffer, SongStructure structure) =>
        RenderNoteSequence(buffer, structure.Style.LeadInstrument, structure.MelodyNotes, structure.Bpm);

    private static void RenderDrumLayer(AudioBuffer buffer, SongStructure structure) =>
        RenderDrumHits(buffer, structure.DrumHits, structure.Bpm);

    private static void RenderNoteSequence(AudioBuffer buffer, Instrument instrument, List<Note> notes, double bpm)
    {
        double beatPosition = 0;
        foreach (var note in notes)
            beatPosition = RenderSingleNote(buffer, instrument, note, beatPosition, bpm);
    }

    private static double RenderSingleNote(AudioBuffer buffer, Instrument instrument, Note note, double beatPosition, double bpm)
    {
        if (!note.IsRest)
            buffer.AddNoteAtBeat(instrument, note.MidiNote, beatPosition, note.DurationInBeats, bpm, note.Velocity);
        return beatPosition + note.DurationInBeats;
    }

    private static void RenderPadNotes(AudioBuffer buffer, Instrument instrument, List<Note> padNotes, int[][] progression, double bpm, int totalBars)
    {
        int noteIndex = 0;
        for (int bar = 0; bar < totalBars; bar++)
            noteIndex = RenderPadBar(buffer, instrument, padNotes, bar, noteIndex, bpm);
    }

    private static int RenderPadBar(AudioBuffer buffer, Instrument instrument, List<Note> padNotes, int bar, int noteIndex, double bpm)
    {
        double barStartBeat = bar * 4.0;
        int chordSize = CountPadChordNotes(padNotes, noteIndex);
        RenderPadChord(buffer, instrument, padNotes, noteIndex, chordSize, barStartBeat, bpm);
        return noteIndex + chordSize;
    }

    private static int CountPadChordNotes(List<Note> padNotes, int startIndex)
    {
        int count = 0;
        while (startIndex + count < padNotes.Count && !padNotes[startIndex + count].IsRest)
            count++;
        return Math.Max(count, 1);
    }

    private static void RenderPadChord(AudioBuffer buffer, Instrument instrument, List<Note> padNotes, int startIndex, int chordSize, double barStartBeat, double bpm)
    {
        for (int i = 0; i < chordSize && startIndex + i < padNotes.Count; i++)
            RenderPadNote(buffer, instrument, padNotes[startIndex + i], barStartBeat, bpm);
    }

    private static void RenderPadNote(AudioBuffer buffer, Instrument instrument, Note note, double barStartBeat, double bpm) =>
        buffer.AddNoteAtBeat(instrument, note.MidiNote, barStartBeat, note.DurationInBeats, bpm, note.Velocity);

    private static void RenderDrumHits(AudioBuffer buffer, List<DrumHit> hits, double bpm)
    {
        foreach (var hit in hits)
            RenderSingleDrumHit(buffer, hit, bpm);
    }

    private static void RenderSingleDrumHit(AudioBuffer buffer, DrumHit hit, double bpm) =>
        buffer.AddNoteAtBeat(hit.DrumInstrument, Pitch.C3, hit.BeatPosition, Duration.Sixteenth, bpm, hit.Velocity);
}
