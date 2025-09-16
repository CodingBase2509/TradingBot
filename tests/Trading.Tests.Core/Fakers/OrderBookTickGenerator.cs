using System;
using System.Collections.Generic;
using Bogus;
using Trading.Core.Domain;

namespace Trading.Tests.Core.Generators;

/// <summary>
/// Generates plausible <see cref="OrderBookTick"/> values (Ask ≥ Bid).
/// Struct-friendly (does not inherit Bogus.Faker&lt;T&gt;).
/// </summary>
public sealed class OrderBookTickGenerator : IGenerator<OrderBookTick>
{
    private readonly string defaultSymbol;
    private Faker faker;
    
    public string DefaultSymbol => defaultSymbol;
    
    public OrderBookTickGenerator(string symbol = "BTCUSDT", int? seed = null)
    {
        defaultSymbol = symbol;
        faker = new Faker();
        
        if (seed.HasValue) 
            SetSeed(seed.Value);
    }
    
    public void SetSeed(int seed)
    {
        Randomizer.Seed = new Random(seed);
        faker = new Faker();
    }

    public OrderBookTick Generate()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var mid = faker.Random.Decimal(10_000m, 70_000m);
        var spread = faker.Random.Decimal(0.5m, 15m);
        var bid = mid - spread / 2m;
        var ask = mid + spread / 2m;

        if (bid < 0m) bid = 0m;
        if (ask < bid) ask = bid;

        return new OrderBookTick(
            TimestampUnixMilliseconds: timestamp,
            Symbol: defaultSymbol,
            BestBidPrice: bid,
            BestAskPrice: ask
        );
    }

    public List<OrderBookTick> GenerateSeries(int count, DateTimeOffset? startUtc = null, TimeSpan? step = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var list = new List<OrderBookTick>(count);
        var t = startUtc ?? DateTimeOffset.UtcNow;
        var dt = step ?? TimeSpan.FromSeconds(1);

        for (var i = 0; i < count; i++)
        {
            var x = Generate();
            var adjusted = x with
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
