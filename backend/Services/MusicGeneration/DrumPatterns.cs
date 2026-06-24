namespace backend.Services.MusicGeneration;

public enum DrumPatternType
{
    FourOnTheFloor,
    Breakbeat,
    HipHop,
    Shuffle,
    Halftime
}

public record DrumHit(Instrument DrumInstrument, double BeatPosition, float Velocity);

public static class DrumPatterns
{
    public static List<DrumHit> Generate(DrumPatternType type, int bars, DeterministicRandom rng)
    {
        var basePattern = GetBasePattern(type);
        return ExpandPattern(basePattern, bars, rng);
    }

    private static List<DrumHit> GetBasePattern(DrumPatternType type) => type switch
    {
        DrumPatternType.FourOnTheFloor => BuildFourOnTheFloor(),
        DrumPatternType.Breakbeat => BuildBreakbeat(),
        DrumPatternType.HipHop => BuildHipHop(),
        DrumPatternType.Shuffle => BuildShuffle(),
        DrumPatternType.Halftime => BuildHalftime(),
        _ => BuildFourOnTheFloor()
    };

    private static List<DrumHit> BuildFourOnTheFloor()
    {
        var hits = new List<DrumHit>();
        AddKickPattern(hits, [0, 1, 2, 3]);
        AddSnarePattern(hits, [1, 3]);
        AddHiHatEighths(hits, 4);
        return hits;
    }

    private static List<DrumHit> BuildBreakbeat()
    {
        var hits = new List<DrumHit>();
        AddKickPattern(hits, [0, 1.5, 2.75]);
        AddSnarePattern(hits, [1, 3, 3.5]);
        AddHiHatEighths(hits, 4);
        return hits;
    }

    private static List<DrumHit> BuildHipHop()
    {
        var hits = new List<DrumHit>();
        AddKickPattern(hits, [0, 2.5]);
        AddSnarePattern(hits, [1, 3]);
        AddHiHatPattern(hits, [0, 0.75, 1, 1.75, 2, 2.75, 3, 3.75]);
        return hits;
    }

    private static List<DrumHit> BuildShuffle()
    {
        var hits = new List<DrumHit>();
        AddKickPattern(hits, [0, 2]);
        AddSnarePattern(hits, [1, 3]);
        AddShuffleHiHats(hits, 4);
        return hits;
    }

    private static List<DrumHit> BuildHalftime()
    {
        var hits = new List<DrumHit>();
        AddKickPattern(hits, [0]);
        AddSnarePattern(hits, [2]);
        AddHiHatEighths(hits, 4);
        return hits;
    }

    private static void AddKickPattern(List<DrumHit> hits, double[] positions)
    {
        foreach (var pos in positions)
            hits.Add(new DrumHit(InstrumentPresets.Kick(), pos, 0.9f));
    }

    private static void AddSnarePattern(List<DrumHit> hits, double[] positions)
    {
        foreach (var pos in positions)
            hits.Add(new DrumHit(InstrumentPresets.Snare(), pos, 0.85f));
    }

    private static void AddHiHatEighths(List<DrumHit> hits, int beatsPerBar)
    {
        for (double beat = 0; beat < beatsPerBar; beat += 0.5)
            hits.Add(new DrumHit(InstrumentPresets.ClosedHiHat(), beat, 0.6f));
    }

    private static void AddHiHatPattern(List<DrumHit> hits, double[] positions)
    {
        foreach (var pos in positions)
            hits.Add(new DrumHit(InstrumentPresets.ClosedHiHat(), pos, 0.55f));
    }

    private static void AddShuffleHiHats(List<DrumHit> hits, int beatsPerBar)
    {
        for (int beat = 0; beat < beatsPerBar; beat++)
            AddSwungPair(hits, beat);
    }

    private static void AddSwungPair(List<DrumHit> hits, int beat)
    {
        hits.Add(new DrumHit(InstrumentPresets.ClosedHiHat(), beat, 0.65f));
        hits.Add(new DrumHit(InstrumentPresets.ClosedHiHat(), beat + 0.67, 0.45f));
    }

    private static List<DrumHit> ExpandPattern(List<DrumHit> singleBar, int bars, DeterministicRandom rng)
    {
        var result = new List<DrumHit>();
        for (int bar = 0; bar < bars; bar++)
            AddBarHits(result, singleBar, bar, rng);
        return result;
    }

    private static void AddBarHits(List<DrumHit> result, List<DrumHit> pattern, int bar, DeterministicRandom rng)
    {
        double barOffset = bar * 4.0;
        foreach (var hit in pattern)
            AddHitWithVariation(result, hit, barOffset, rng);
    }

    private static void AddHitWithVariation(List<DrumHit> result, DrumHit hit, double barOffset, DeterministicRandom rng)
    {
        float velocityVariation = rng.NextFloatRange(0.85f, 1.0f);
        result.Add(hit with { BeatPosition = hit.BeatPosition + barOffset, Velocity = hit.Velocity * velocityVariation });
    }
}
