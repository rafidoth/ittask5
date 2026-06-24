using System;

namespace backend.Services.MusicGeneration;

public static class Pitch
{
    public const int C2 = 36;
    public const int C3 = 48;
    public const int C4 = 60;
    public const int C5 = 72;
    public const int A4 = 69;

    public static double MidiToFrequency(int midiNote) =>
        440.0 * Math.Pow(2.0, (midiNote - 69) / 12.0);

    public static int FrequencyToMidi(double frequency) =>
        (int)Math.Round(69.0 + 12.0 * Math.Log2(frequency / 440.0));

    public static int Transpose(int midiNote, int semitones) =>
        midiNote + semitones;

    public static int OctaveShift(int midiNote, int octaves) =>
        midiNote + octaves * 12;

    public static int Clamp(int midiNote, int low, int high) =>
        Math.Clamp(midiNote, low, high);
}
