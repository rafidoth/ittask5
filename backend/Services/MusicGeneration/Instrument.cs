namespace backend.Services.MusicGeneration;

public class Instrument
{
    public WaveformType PrimaryWaveform { get; init; }
    public AdsrEnvelope Envelope { get; init; } = AdsrEnvelope.Pluck;
    public float BaseVolume { get; init; } = 0.5f;
    public float DetuneAmount { get; init; } = 0f;
    public float NoiseAmount { get; init; } = 0f;
    public AdsrEnvelope NoiseEnvelope { get; init; } = AdsrEnvelope.Percussive;
    public double PitchDecayRate { get; init; } = 0;
    public double PitchDecayDepth { get; init; } = 0;

    public float GenerateSample(double phase, double detunePhase, double time, double noteDuration, float velocity, uint sampleIndex)
    {
        float toneComponent = ComputeTone(phase, detunePhase);
        float noiseComponent = ComputeNoise(time, noteDuration, sampleIndex);
        float mixed = MixToneAndNoise(toneComponent, noiseComponent);
        return ApplyEnvelopeAndVolume(mixed, time, noteDuration, velocity);
    }

    private float ComputeTone(double phase, double detunePhase) =>
        DetuneAmount > 0 ? BlendDetuned(phase, detunePhase) : Waveforms.Generate(PrimaryWaveform, phase);

    private float BlendDetuned(double phase, double detunePhase) =>
        (Waveforms.Generate(PrimaryWaveform, phase) + Waveforms.Generate(PrimaryWaveform, detunePhase)) * 0.5f;

    private float ComputeNoise(double time, double noteDuration, uint sampleIndex) =>
        Waveforms.WhiteNoise(sampleIndex) * (float)NoiseEnvelope.GetAmplitude(time, noteDuration) * NoiseAmount;

    private float MixToneAndNoise(float tone, float noise) =>
        tone * (1f - NoiseAmount) + noise;

    private float ApplyEnvelopeAndVolume(float sample, double time, double noteDuration, float velocity) =>
        sample * (float)Envelope.GetAmplitude(time, noteDuration) * BaseVolume * velocity;

    public double CalculateFrequency(double baseFrequency, double time) =>
        PitchDecayRate > 0 ? ApplyPitchDecay(baseFrequency, time) : baseFrequency;

    private double ApplyPitchDecay(double baseFrequency, double time) =>
        baseFrequency * Math.Pow(2.0, PitchDecayDepth * Math.Exp(-PitchDecayRate * time) / 12.0);

    public double DetuneRatio =>
        1.0 + DetuneAmount * 0.0001;
}
