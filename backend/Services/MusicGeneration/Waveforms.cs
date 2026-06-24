namespace backend.Services.MusicGeneration;

public enum WaveformType
{
    Sine,
    Square,
    Sawtooth,
    Triangle,
    WhiteNoise
}

public static class Waveforms
{
    public static float Generate(WaveformType type, double phase, uint sampleIndex = 0) => type switch
    {
        WaveformType.Sine => Sine(phase),
        WaveformType.Square => Square(phase),
        WaveformType.Sawtooth => Sawtooth(phase),
        WaveformType.Triangle => Triangle(phase),
        WaveformType.WhiteNoise => WhiteNoise(sampleIndex),
        _ => Sine(phase)
    };

    public static float Sine(double phase) =>
        (float)Math.Sin(2.0 * Math.PI * phase);

    public static float Square(double phase) =>
        phase % 1.0 < 0.5 ? 1f : -1f;

    public static float Sawtooth(double phase) =>
        (float)(2.0 * (phase % 1.0) - 1.0);

    public static float Triangle(double phase) =>
        (float)(2.0 * Math.Abs(2.0 * (phase % 1.0) - 1.0) - 1.0);

    public static float WhiteNoise(uint sampleIndex) =>
        HashToFloat(HashSample(sampleIndex));

    private static uint HashSample(uint x)
    {
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        return (x >> 16) ^ x;
    }

    private static float HashToFloat(uint hash) =>
        (hash & 0xFFFF) / 32768f - 1f;
}
