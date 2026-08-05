namespace TradingPlatform.Market.Calendars.Catalog;

internal sealed class InMemoryMarketCalendarCatalog : IMarketCalendarCatalog
{
    private readonly IReadOnlyList<MarketCalendar> _calendars;
    private readonly Dictionary<Guid, MarketCalendar> _calendarsById = new();

    public InMemoryMarketCalendarCatalog()
        : this([])
    {
    }

    public InMemoryMarketCalendarCatalog(IEnumerable<MarketCalendar> calendars)
    {
        ArgumentNullException.ThrowIfNull(calendars);

        var snapshot = calendars.ToArray();
        if (snapshot.Any(calendar => calendar is null))
        {
            throw new ArgumentException("The catalog must not contain null calendars.", nameof(calendars));
        }

        foreach (var calendar in snapshot)
        {
            if (!_calendarsById.TryAdd(calendar.Id, calendar))
            {
                throw new ArgumentException(
                    $"The market calendar ID '{calendar.Id}' is assigned more than once.",
                    nameof(calendars));
            }
        }

        _calendars = Array.AsReadOnly(snapshot);
    }

    public Task<MarketCalendar?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("A market calendar ID must not be empty.", nameof(id));
        }

        cancellationToken.ThrowIfCancellationRequested();
        _calendarsById.TryGetValue(id, out var calendar);
        return Task.FromResult(calendar);
    }

    public Task<IReadOnlyList<MarketCalendar>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_calendars);
    }
}
