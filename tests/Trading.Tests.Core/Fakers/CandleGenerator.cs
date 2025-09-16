using Bogus;
using Trading.Core.Domain;

namespace Trading.Tests.Core.Generators;

/// <summary>
/// Generates plausible <see cref="Candle"/> values (OHLCV consistent, IsClosedBar = true).
/// Struct-friendly (does not inherit Bogus.Faker&lt;T&gt;).
/// </summary>
public sealed class CandleGenerator : IGenerator<Candle>
{
    private readonly string defaultSymbol;
    private readonly TimeSpan defaultInterval;
    private Faker faker;
    
    public string DefaultSymbol => defaultSymbol;
    public TimeSpan DefaultInterval => defaultInterval;
    
    public CandleGenerator(string symbol = "BTCUSDT", TimeSpan? interval = null, int? seed = null)
    {
        defaultSymbol = symbol;
        defaultInterval = interval ?? TimeSpan.FromMinutes(15);
        
        faker = new Faker();

        if (seed.HasValue) 
            SetSeed(seed.Value);
    }
    
    public void SetSeed(int seed)
    {
        Randomizer.Seed = new Random(seed);
        faker = new Faker();
    }

    public Candle Generate()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var close = faker.Random.Decimal(10_000m, 70_000m);
        var delta = faker.Random.Decimal(-150m, 150m);
        var open = close + delta;
        var wick = faker.Random.Decimal(0m, 300m);
        var low = Math.Min(open, close) - wick / 2m;
        var high = Math.Max(open, close) + wick / 2m;
        if (low < 0m) low = 0m;

        var volume = faker.Random.Decimal(0.01m, 500m);

        return new Candle(
            TimestampUnixMilliseconds: timestamp,
            Symbol: defaultSymbol,
            OpenPrice: open,
            HighPrice: high,
            LowPrice: low,
            ClosePrice: close,
            Volume: volume,
            IsClosedBar: true
        );
    }

    public List<Candle> GenerateSeries(int count, DateTimeOffset? startUtc = null, TimeSpan? step = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var list = new List<Candle>(count);
        var t = startUtc ?? DateTimeOffset.UtcNow;
        var dt = step ?? defaultInterval;

        for (var i = 0; i < count; i++)
        {
            var c = Generate();
            var adjusted = c with
            {
                TimestampUnixMilliseconds = t.ToUnixTimeMilliseconds(),
                Symbol = defaultSymbol
            };
            list.Add(adjusted);
            t = t.Add(dt);
        }

        return list;
    }
}
