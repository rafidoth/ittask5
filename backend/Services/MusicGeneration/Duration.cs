namespace backend.Services.MusicGeneration;

public static class Duration
{
    public const double Whole = 4.0;
    public const double Half = 2.0;
    public const double Quarter = 1.0;
    public const double Eighth = 0.5;
    public const double Sixteenth = 0.25;
    public const double DottedHalf = 3.0;
    public const double DottedQuarter = 1.5;
    public const double DottedEighth = 0.75;
    public const double TripletQuarter = 2.0 / 3.0;
    public const double TripletEighth = 1.0 / 3.0;

    public static double BeatsToSeconds(double beats, double bpm) =>
        beats * 60.0 / bpm;

    public static int BeatsToSamples(double beats, double bpm, int sampleRate) =>
        (int)(BeatsToSeconds(beats, bpm) * sampleRate);

    public static double SamplesToBeats(int samples, double bpm, int sampleRate) =>
        (double)samples / sampleRate * bpm / 60.0;

    public static double BarsToBeats(int bars, int beatsPerBar = 4) =>
        bars * beatsPerBar;
}
