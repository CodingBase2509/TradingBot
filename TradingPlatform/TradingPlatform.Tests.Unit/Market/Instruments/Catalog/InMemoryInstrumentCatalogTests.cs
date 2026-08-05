using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Catalog;
using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Instruments.Catalog;

public sealed class InMemoryInstrumentCatalogTests
{
    [Fact]
    public async Task InstrumentCanBeFoundById()
    {
        var expected = TestInstruments.Mes();
        var catalog = new InMemoryInstrumentCatalog([expected]);

        var actual = await catalog.GetAsync(expected.Id);

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task InstrumentCanBeFoundByProviderSymbol()
    {
        var expected = TestInstruments.Mes();
        var providerSymbol = expected.ProviderSymbols[0];
        var catalog = new InMemoryInstrumentCatalog([expected]);

        var actual = await catalog.GetAsync(providerSymbol);

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task UnknownInstrumentReturnsNull()
    {
        var catalog = new InMemoryInstrumentCatalog([TestInstruments.Mes()]);

        var actual = await catalog.GetAsync(new InstrumentId("UNKNOWN"));

        Assert.Null(actual);
    }

    [Fact]
    public async Task CatalogUsesAnImmutableSnapshotOfTheSource()
    {
        var source = new List<InstrumentDefinition> { TestInstruments.Mes() };
        var catalog = new InMemoryInstrumentCatalog(source);

        source.Clear();
        var instruments = await catalog.GetAllAsync();

        Assert.Single(instruments);
        Assert.IsNotType<List<InstrumentDefinition>>(instruments);
    }

    [Fact]
    public void DuplicateInstrumentIdsAreRejected()
    {
        var instruments = new[]
        {
            CreateInstrument("DUPLICATE", "FIRST"),
            CreateInstrument("DUPLICATE", "SECOND"),
        };

        Assert.Throws<ArgumentException>(() => new InMemoryInstrumentCatalog(instruments));
    }

    [Fact]
    public void DuplicateProviderSymbolsAreRejected()
    {
        var instruments = new[]
        {
            CreateInstrument("FIRST", "SHARED"),
            CreateInstrument("SECOND", "SHARED"),
        };

        Assert.Throws<ArgumentException>(() => new InMemoryInstrumentCatalog(instruments));
    }

    [Fact]
    public async Task CancelledLookupIsRejected()
    {
        var catalog = new InMemoryInstrumentCatalog([TestInstruments.Mes()]);
        var cancellationToken = new CancellationToken(canceled: true);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => catalog.GetAllAsync(cancellationToken));
    }

    private static InstrumentDefinition CreateInstrument(string id, string symbol) => new(
        new InstrumentId(id),
        id,
        InstrumentType.Equity,
        "XNAS",
        "USD",
        tickSize: 0.01m,
        tickValue: 0.01m,
        minimumQuantity: 1m,
        new InstrumentCapabilities(
            SupportsLong: true,
            SupportsShort: true,
            HasExpiringContracts: false,
            RequiresRollover: false),
        [new ProviderSymbol(ProviderKind.MarketData, "TestProvider", symbol)],
        Guid.Parse("33333333-3333-7333-8333-333333333333"));
}
