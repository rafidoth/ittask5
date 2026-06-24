namespace backend.Services.MusicGeneration;

public record AdsrEnvelope(double Attack, double Decay, double SustainLevel, double Release)
{
    public double GetAmplitude(double time, double noteDuration)
    {
        if (time < 0) return 0;
        if (time < Attack) return ComputeAttack(time);
        if (time < Attack + Decay) return ComputeDecay(time);
        if (time < noteDuration) return SustainLevel;
        return ComputeRelease(time, noteDuration);
    }

    private double ComputeAttack(double time) =>
        time / Math.Max(Attack, 0.001);

    private double ComputeDecay(double time) =>
        1.0 - (1.0 - SustainLevel) * ((time - Attack) / Math.Max(Decay, 0.001));

    private double ComputeRelease(double time, double noteDuration) =>
        Math.Max(0, SustainLevel * (1.0 - (time - noteDuration) / Math.Max(Release, 0.001)));

    public static AdsrEnvelope Pluck => new(0.005, 0.1, 0.3, 0.2);
    public static AdsrEnvelope Pad => new(0.4, 0.3, 0.7, 0.8);
    public static AdsrEnvelope Organ => new(0.01, 0.05, 0.9, 0.05);
    public static AdsrEnvelope Percussive => new(0.001, 0.15, 0.0, 0.1);
    public static AdsrEnvelope ShortPercussive => new(0.001, 0.05, 0.0, 0.05);
    public static AdsrEnvelope Piano => new(0.005, 0.5, 0.4, 0.3);
    public static AdsrEnvelope Soft => new(0.2, 0.2, 0.6, 0.5);
    public static AdsrEnvelope Bass => new(0.01, 0.2, 0.6, 0.15);
}
