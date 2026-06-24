namespace backend.Services.MusicGeneration;

public static class Effects
{
    public static void ApplyDelay(float[] buffer, int delaySamples, float feedback, float mix)
    {
        var delayed = CreateDelayBuffer(buffer, delaySamples, feedback);
        MixBuffers(buffer, delayed, mix);
    }

    private static float[] CreateDelayBuffer(float[] source, int delaySamples, float feedback)
    {
        var result = new float[source.Length];
        for (int i = 0; i < source.Length; i++)
            result[i] = ComputeDelayedSample(source, result, i, delaySamples, feedback);
        return result;
    }

    private static float ComputeDelayedSample(float[] source, float[] delayed, int index, int delaySamples, float feedback) =>
        source[index] + (index >= delaySamples ? delayed[index - delaySamples] * feedback : 0f);

    private static void MixBuffers(float[] target, float[] wet, float mix)
    {
        for (int i = 0; i < target.Length; i++)
            target[i] = target[i] * (1f - mix) + wet[i] * mix;
    }

    public static void ApplyReverb(float[] buffer, float roomSize, float dampening, float mix)
    {
        var wet = new float[buffer.Length];
        ApplyCombFilters(buffer, wet, roomSize, dampening);
        ApplyAllPassFilters(wet);
        MixBuffers(buffer, wet, mix);
    }

    private static readonly int[] CombDelays = [1116, 1188, 1277, 1356, 1422, 1491, 1557, 1617];
    private static readonly int[] AllPassDelays = [556, 441, 341, 225];

    private static void ApplyCombFilters(float[] input, float[] output, float roomSize, float dampening)
    {
        foreach (var delay in CombDelays)
            ApplySingleCombFilter(input, output, (int)(delay * roomSize), dampening);
    }

    private static void ApplySingleCombFilter(float[] input, float[] output, int delay, float dampening)
    {
        float filterStore = 0;
        for (int i = delay; i < input.Length; i++)
            output[i] += ProcessCombSample(input, output, i, delay, dampening, ref filterStore);
    }

    private static float ProcessCombSample(float[] input, float[] output, int index, int delay, float dampening, ref float filterStore)
    {
        filterStore = output[index - delay] * (1f - dampening) + filterStore * dampening;
        return (input[index] + filterStore * 0.5f) / CombDelays.Length;
    }

    private static void ApplyAllPassFilters(float[] buffer)
    {
        foreach (var delay in AllPassDelays)
            ApplySingleAllPassFilter(buffer, delay);
    }

    private static void ApplySingleAllPassFilter(float[] buffer, int delay)
    {
        var delayLine = new float[delay];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = ProcessAllPassSample(buffer, delayLine, i, delay);
    }

    private static float ProcessAllPassSample(float[] buffer, float[] delayLine, int index, int delay)
    {
        float delayed = delayLine[index % delay];
        float input = buffer[index];
        float output = delayed - 0.5f * input;
        delayLine[index % delay] = input + 0.5f * output;
        return output;
    }

    public static void Normalize(float[] buffer)
    {
        float peak = FindPeakAmplitude(buffer);
        if (peak > 0.001f)
            ScaleBuffer(buffer, 0.95f / peak);
    }

    private static float FindPeakAmplitude(float[] buffer) =>
        buffer.Length > 0 ? buffer.Max(s => Math.Abs(s)) : 0f;

    private static void ScaleBuffer(float[] buffer, float scale)
    {
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] *= scale;
    }

    public static void ApplyFadeOut(float[] buffer, int fadeSamples)
    {
        int fadeStart = Math.Max(0, buffer.Length - fadeSamples);
        for (int i = fadeStart; i < buffer.Length; i++)
            buffer[i] *= CalculateFadeMultiplier(i, fadeStart, fadeSamples);
    }

    private static float CalculateFadeMultiplier(int index, int fadeStart, int fadeSamples) =>
        1f - (float)(index - fadeStart) / fadeSamples;
}
