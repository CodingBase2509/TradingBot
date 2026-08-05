using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Calendars.Catalog;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Calendars.Catalog;

public sealed class InMemoryMarketCalendarCatalogTests
{
    [Fact]
    public async Task CalendarCanBeFoundById()
    {
        var expected = TestMarketCalendars.Nyse();
        var catalog = new InMemoryMarketCalendarCatalog([expected]);

        var actual = await catalog.GetAsync(expected.Id);

        Assert.Same(expected, actual);
    }

    [Fact]
    public async Task UnknownCalendarReturnsNull()
    {
        var catalog = new InMemoryMarketCalendarCatalog([TestMarketCalendars.Nyse()]);

        var actual = await catalog.GetAsync(Guid.Parse("55555555-5555-7555-8555-555555555555"));

        Assert.Null(actual);
    }

    [Fact]
    public async Task CatalogUsesAnImmutableSnapshotOfTheSource()
    {
        var source = new List<MarketCalendar> { TestMarketCalendars.Nyse() };
        var catalog = new InMemoryMarketCalendarCatalog(source);

        source.Clear();
        var calendars = await catalog.GetAllAsync();

        Assert.Single(calendars);
        Assert.IsNotType<List<MarketCalendar>>(calendars);
    }

    [Fact]
    public void DuplicateCalendarIdsAreRejected()
    {
        var calendar = TestMarketCalendars.Nyse();

        Assert.Throws<ArgumentException>(() => new InMemoryMarketCalendarCatalog([calendar, calendar]));
    }

    [Fact]
    public async Task CancelledLookupIsRejected()
    {
        var catalog = new InMemoryMarketCalendarCatalog([TestMarketCalendars.Nyse()]);
        var cancellationToken = new CancellationToken(canceled: true);

        await Assert.ThrowsAsync<OperationCanceledException>(() => catalog.GetAllAsync(cancellationToken));
    }
}
