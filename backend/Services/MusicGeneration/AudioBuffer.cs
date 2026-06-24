namespace backend.Services.MusicGeneration;

public class AudioBuffer
{
    public const int SampleRate = 44100;
    private readonly float[] _samples;

    public AudioBuffer(int totalSamples) => _samples = new float[totalSamples];

    public AudioBuffer(double durationSeconds) => _samples = new float[(int)(durationSeconds * SampleRate)];

    public int Length => _samples.Length;

    public float[] GetSamples() => _samples;

    public void MixSample(int index, float value)
    {
        if (index >= 0 && index < _samples.Length)
            _samples[index] += value;
    }

    public void AddNote(Instrument instrument, int midiNote, int startSample, int durationSamples, float velocity)
    {
        double frequency = Pitch.MidiToFrequency(midiNote);
        int totalSamples = CalculateNoteTotalSamples(instrument, durationSamples);
        RenderNote(instrument, frequency, startSample, durationSamples, totalSamples, velocity);
    }

    private int CalculateNoteTotalSamples(Instrument instrument, int durationSamples) =>
        durationSamples + (int)(instrument.Envelope.Release * SampleRate);

    private void RenderNote(Instrument instrument, double frequency, int startSample, int durationSamples, int totalSamples, float velocity)
    {
        double phase = 0, detunePhase = 0;
        double durationSeconds = (double)durationSamples / SampleRate;
        for (int i = 0; i < totalSamples && startSample + i < _samples.Length; i++)
            RenderSingleNoteSample(instrument, ref phase, ref detunePhase, frequency, startSample + i, i, durationSeconds, velocity);
    }

    private void RenderSingleNoteSample(Instrument instrument, ref double phase, ref double detunePhase, double baseFrequency, int bufferIndex, int sampleOffset, double durationSeconds, float velocity)
    {
        double time = (double)sampleOffset / SampleRate;
        double adjustedFrequency = instrument.CalculateFrequency(baseFrequency, time);
        phase = AdvancePhase(phase, adjustedFrequency);
        detunePhase = AdvancePhase(detunePhase, adjustedFrequency * instrument.DetuneRatio);
        MixSample(bufferIndex, instrument.GenerateSample(phase, detunePhase, time, durationSeconds, velocity, (uint)bufferIndex));
    }

    private static double AdvancePhase(double currentPhase, double frequency) =>
        (currentPhase + frequency / SampleRate) % 1.0;

    public void AddNoteAtBeat(Instrument instrument, int midiNote, double beatPosition, double beatDuration, double bpm, float velocity)
    {
        int startSample = Duration.BeatsToSamples(beatPosition, bpm, SampleRate);
        int durationSamples = Duration.BeatsToSamples(beatDuration, bpm, SampleRate);
        AddNote(instrument, midiNote, startSample, durationSamples, velocity);
    }
}
