namespace backend.Services.MusicGeneration;

public record Note(int MidiNote, double DurationInBeats, float Velocity, bool IsRest)
{
    public static Note CreateRest(double durationInBeats) =>
        new(0, durationInBeats, 0f, true);

    public static Note Create(int midiNote, double durationInBeats, float velocity = 0.8f) =>
        new(midiNote, durationInBeats, velocity, false);

    public static Note CreateWithRandomVelocity(int midiNote, double durationInBeats, DeterministicRandom rng) =>
        new(midiNote, durationInBeats, rng.NextFloatRange(0.6f, 1.0f), false);
}
