using System;
using System.Collections.Generic;

namespace backend.Services.MusicGeneration;

public class DeterministicRandom
{
    private readonly Random _rng;

    public DeterministicRandom(int seed)
    {
        _rng = new Random(seed);
    }

    public uint NextUInt() => (uint)_rng.NextInt64(0, uint.MaxValue + 1L);

    public int NextInt(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);

    public float NextFloat() => _rng.NextSingle();

    public double NextDouble() => _rng.NextDouble();

    public bool NextBool() => _rng.Next(2) == 0;

    public T Choose<T>(IReadOnlyList<T> items) => items[NextInt(0, items.Count)];

    public double NextGaussian()
    {
        double u1 = 1.0 - NextDouble();
        double u2 = NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
    }

    public float NextFloatRange(float min, float max) => min + NextFloat() * (max - min);

    public double NextDoubleRange(double min, double max) => min + NextDouble() * (max - min);

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
