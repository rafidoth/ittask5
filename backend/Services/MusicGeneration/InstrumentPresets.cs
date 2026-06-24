namespace backend.Services.MusicGeneration;

public static class InstrumentPresets
{
    public static Instrument DeepSubBass() => new()
    {
        PrimaryWaveform = WaveformType.Sine,
        Envelope = AdsrEnvelope.Bass,
        BaseVolume = 0.7f
    };

    public static Instrument SynthBass() => new()
    {
        PrimaryWaveform = WaveformType.Square,
        Envelope = AdsrEnvelope.Bass,
        BaseVolume = 0.55f,
        DetuneAmount = 5f
    };

    public static Instrument FunkBass() => new()
    {
        PrimaryWaveform = WaveformType.Sawtooth,
        Envelope = AdsrEnvelope.Pluck,
        BaseVolume = 0.5f,
        DetuneAmount = 3f
    };

    public static Instrument SquareLead() => new()
    {
        PrimaryWaveform = WaveformType.Square,
        Envelope = AdsrEnvelope.Organ,
        BaseVolume = 0.4f,
        DetuneAmount = 8f
    };

    public static Instrument SawLead() => new()
    {
        PrimaryWaveform = WaveformType.Sawtooth,
        Envelope = AdsrEnvelope.Pluck,
        BaseVolume = 0.4f,
        DetuneAmount = 12f
    };

    public static Instrument PluckyArpeggiator() => new()
    {
        PrimaryWaveform = WaveformType.Triangle,
        Envelope = AdsrEnvelope.Pluck,
        BaseVolume = 0.35f,
        DetuneAmount = 6f
    };

    public static Instrument AiryPad() => new()
    {
        PrimaryWaveform = WaveformType.Sine,
        Envelope = AdsrEnvelope.Pad,
        BaseVolume = 0.25f,
        DetuneAmount = 15f,
        NoiseAmount = 0.05f,
        NoiseEnvelope = AdsrEnvelope.Pad
    };

    public static Instrument WarmPad() => new()
    {
        PrimaryWaveform = WaveformType.Triangle,
        Envelope = AdsrEnvelope.Pad,
        BaseVolume = 0.3f,
        DetuneAmount = 20f
    };

    public static Instrument StringEnsemble() => new()
    {
        PrimaryWaveform = WaveformType.Sawtooth,
        Envelope = AdsrEnvelope.Pad,
        BaseVolume = 0.25f,
        DetuneAmount = 25f
    };

    public static Instrument ElectricPiano() => new()
    {
        PrimaryWaveform = WaveformType.Sine,
        Envelope = AdsrEnvelope.Piano,
        BaseVolume = 0.45f,
        DetuneAmount = 3f
    };

    public static Instrument Organ() => new()
    {
        PrimaryWaveform = WaveformType.Square,
        Envelope = AdsrEnvelope.Organ,
        BaseVolume = 0.35f
    };

    public static Instrument Bells() => new()
    {
        PrimaryWaveform = WaveformType.Sine,
        Envelope = AdsrEnvelope.Pluck,
        BaseVolume = 0.3f,
        DetuneAmount = 30f
    };

    public static Instrument Kick() => new()
    {
        PrimaryWaveform = WaveformType.Sine,
        Envelope = AdsrEnvelope.Percussive,
        BaseVolume = 0.8f,
        PitchDecayRate = 40,
        PitchDecayDepth = 36
    };

    public static Instrument Snare() => new()
    {
        PrimaryWaveform = WaveformType.Triangle,
        Envelope = AdsrEnvelope.Percussive,
        BaseVolume = 0.65f,
        NoiseAmount = 0.7f,
        NoiseEnvelope = AdsrEnvelope.Percussive
    };

    public static Instrument ClosedHiHat() => new()
    {
        PrimaryWaveform = WaveformType.Square,
        Envelope = AdsrEnvelope.ShortPercussive,
        BaseVolume = 0.35f,
        NoiseAmount = 0.9f,
        NoiseEnvelope = AdsrEnvelope.ShortPercussive
    };

    public static Instrument OpenHiHat() => new()
    {
        PrimaryWaveform = WaveformType.Square,
        Envelope = new AdsrEnvelope(0.001, 0.2, 0.1, 0.3),
        BaseVolume = 0.3f,
        NoiseAmount = 0.9f,
        NoiseEnvelope = new AdsrEnvelope(0.001, 0.2, 0.1, 0.3)
    };

    public static Instrument Clap() => new()
    {
        PrimaryWaveform = WaveformType.WhiteNoise,
        Envelope = new AdsrEnvelope(0.001, 0.08, 0.0, 0.15),
        BaseVolume = 0.5f,
        NoiseAmount = 0.95f,
        NoiseEnvelope = new AdsrEnvelope(0.001, 0.08, 0.0, 0.15)
    };

    public static Instrument RimShot() => new()
    {
        PrimaryWaveform = WaveformType.Square,
        Envelope = AdsrEnvelope.ShortPercussive,
        BaseVolume = 0.45f,
        NoiseAmount = 0.4f,
        NoiseEnvelope = AdsrEnvelope.ShortPercussive,
        PitchDecayRate = 60,
        PitchDecayDepth = 12
    };

    public static Instrument Tom() => new()
    {
        PrimaryWaveform = WaveformType.Sine,
        Envelope = new AdsrEnvelope(0.001, 0.2, 0.0, 0.2),
        BaseVolume = 0.6f,
        PitchDecayRate = 30,
        PitchDecayDepth = 24
    };

    public static Instrument CrashCymbal() => new()
    {
        PrimaryWaveform = WaveformType.WhiteNoise,
        Envelope = new AdsrEnvelope(0.001, 0.8, 0.1, 1.0),
        BaseVolume = 0.25f,
        NoiseAmount = 0.95f,
        NoiseEnvelope = new AdsrEnvelope(0.001, 0.8, 0.1, 1.0)
    };

    public static Instrument RideCymbal() => new()
    {
        PrimaryWaveform = WaveformType.Triangle,
        Envelope = new AdsrEnvelope(0.001, 0.4, 0.15, 0.5),
        BaseVolume = 0.25f,
        NoiseAmount = 0.8f,
        NoiseEnvelope = new AdsrEnvelope(0.001, 0.4, 0.15, 0.5)
    };
}
