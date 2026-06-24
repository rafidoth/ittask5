namespace backend.Services.MusicGeneration;

public enum SongStyleType
{
    PopRock,
    EdmSynthwave,
    AmbientLofi
}

public class SongStyle
{
    public SongStyleType Type { get; init; }
    public int MinTempo { get; init; }
    public int MaxTempo { get; init; }
    public ScaleType[] PreferredScales { get; init; } = [];
    public Instrument BassInstrument { get; init; } = InstrumentPresets.DeepSubBass();
    public Instrument LeadInstrument { get; init; } = InstrumentPresets.SquareLead();
    public Instrument PadInstrument { get; init; } = InstrumentPresets.WarmPad();
    public Instrument ArpInstrument { get; init; } = InstrumentPresets.PluckyArpeggiator();
    public float DelayMix { get; init; } = 0.15f;
    public float ReverbMix { get; init; } = 0.2f;
    public float DelayFeedback { get; init; } = 0.3f;
    public double ReverbRoomSize { get; init; } = 1.0;
    public float ReverbDampening { get; init; } = 0.5f;
    public double NoteDensity { get; init; } = 0.7;
    public double RestProbability { get; init; } = 0.15;
    public int SongBars { get; init; } = 16;
    public DrumPatternType PreferredDrumPattern { get; init; } = DrumPatternType.FourOnTheFloor;

    public static SongStyle Create(SongStyleType type, DeterministicRandom rng) => type switch
    {
        SongStyleType.PopRock => CreatePopRock(rng),
        SongStyleType.EdmSynthwave => CreateEdmSynthwave(rng),
        SongStyleType.AmbientLofi => CreateAmbientLofi(rng),
        _ => CreatePopRock(rng)
    };

    private static SongStyle CreatePopRock(DeterministicRandom rng) => new()
    {
        Type = SongStyleType.PopRock,
        MinTempo = 110,
        MaxTempo = 140,
        PreferredScales = [ScaleType.Major, ScaleType.Mixolydian, ScaleType.Pentatonic],
        BassInstrument = rng.Choose([InstrumentPresets.FunkBass(), InstrumentPresets.SynthBass()]),
        LeadInstrument = rng.Choose([InstrumentPresets.SawLead(), InstrumentPresets.SquareLead()]),
        PadInstrument = rng.Choose([InstrumentPresets.StringEnsemble(), InstrumentPresets.WarmPad()]),
        ArpInstrument = rng.Choose([InstrumentPresets.ElectricPiano(), InstrumentPresets.PluckyArpeggiator()]),
        DelayMix = 0.1f,
        ReverbMix = 0.15f,
        DelayFeedback = 0.25f,
        ReverbRoomSize = 0.8,
        ReverbDampening = 0.6f,
        NoteDensity = rng.NextDoubleRange(0.6, 0.85),
        RestProbability = rng.NextDoubleRange(0.1, 0.2),
        SongBars = 16,
        PreferredDrumPattern = rng.Choose([DrumPatternType.FourOnTheFloor, DrumPatternType.Breakbeat])
    };

    private static SongStyle CreateEdmSynthwave(DeterministicRandom rng) => new()
    {
        Type = SongStyleType.EdmSynthwave,
        MinTempo = 120,
        MaxTempo = 150,
        PreferredScales = [ScaleType.NaturalMinor, ScaleType.Dorian, ScaleType.MinorPentatonic],
        BassInstrument = rng.Choose([InstrumentPresets.SynthBass(), InstrumentPresets.DeepSubBass()]),
        LeadInstrument = rng.Choose([InstrumentPresets.SquareLead(), InstrumentPresets.SawLead()]),
        PadInstrument = rng.Choose([InstrumentPresets.WarmPad(), InstrumentPresets.StringEnsemble()]),
        ArpInstrument = rng.Choose([InstrumentPresets.PluckyArpeggiator(), InstrumentPresets.SquareLead()]),
        DelayMix = 0.2f,
        ReverbMix = 0.25f,
        DelayFeedback = 0.35f,
        ReverbRoomSize = 1.0,
        ReverbDampening = 0.4f,
        NoteDensity = rng.NextDoubleRange(0.75, 0.95),
        RestProbability = rng.NextDoubleRange(0.05, 0.15),
        SongBars = 16,
        PreferredDrumPattern = rng.Choose([DrumPatternType.Breakbeat, DrumPatternType.FourOnTheFloor])
    };

    private static SongStyle CreateAmbientLofi(DeterministicRandom rng) => new()
    {
        Type = SongStyleType.AmbientLofi,
        MinTempo = 70,
        MaxTempo = 95,
        PreferredScales = [ScaleType.Pentatonic, ScaleType.Blues, ScaleType.HarmonicMinor, ScaleType.Dorian],
        BassInstrument = rng.Choose([InstrumentPresets.DeepSubBass(), InstrumentPresets.FunkBass()]),
        LeadInstrument = rng.Choose([InstrumentPresets.Bells(), InstrumentPresets.ElectricPiano()]),
        PadInstrument = rng.Choose([InstrumentPresets.AiryPad(), InstrumentPresets.WarmPad()]),
        ArpInstrument = rng.Choose([InstrumentPresets.ElectricPiano(), InstrumentPresets.Bells()]),
        DelayMix = 0.3f,
        ReverbMix = 0.4f,
        DelayFeedback = 0.45f,
        ReverbRoomSize = 1.3,
        ReverbDampening = 0.3f,
        NoteDensity = rng.NextDoubleRange(0.4, 0.6),
        RestProbability = rng.NextDoubleRange(0.2, 0.4),
        SongBars = 12,
        PreferredDrumPattern = rng.Choose([DrumPatternType.HipHop, DrumPatternType.Shuffle, DrumPatternType.Halftime])
    };
}
