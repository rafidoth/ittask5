using System;
using System.IO;

namespace backend.Services.MusicGeneration;

public static class WavExporter
{
    public static byte[] Export(float[] samples)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        int dataSize = samples.Length * 2;
        WriteWavHeader(writer, dataSize);
        WriteSamples(writer, samples);
        return stream.ToArray();
    }

    private static void WriteWavHeader(BinaryWriter writer, int dataSize)
    {
        writer.Write("RIFF"u8.ToArray());
        writer.Write(36 + dataSize);
        writer.Write("WAVE"u8.ToArray());
        writer.Write("fmt "u8.ToArray());
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(AudioBuffer.SampleRate);
        writer.Write(AudioBuffer.SampleRate * 2);
        writer.Write((short)2);
        writer.Write((short)16);
        writer.Write("data"u8.ToArray());
        writer.Write(dataSize);
    }

    private static void WriteSamples(BinaryWriter writer, float[] samples)
    {
        foreach (float sample in samples)
        {
            float clamped = Math.Clamp(sample, -1f, 1f);
            writer.Write((short)(clamped * short.MaxValue));
        }
    }
}
