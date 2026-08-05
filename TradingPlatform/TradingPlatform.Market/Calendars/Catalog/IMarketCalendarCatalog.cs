namespace TradingPlatform.Market.Calendars.Catalog;

public interface IMarketCalendarCatalog
{
    Task<MarketCalendar?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MarketCalendar>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
