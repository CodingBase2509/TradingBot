using NodaTime;

namespace TradingPlatform.Market.Calendars;

public sealed record MarketCalendar
{
    private readonly Dictionary<IsoDayOfWeek, IReadOnlyList<MarketSessionSchedule>> _weeklySessions;
    private readonly Dictionary<LocalDate, IReadOnlyList<MarketSessionSchedule>> _dateOverrides;

    internal MarketCalendar(
        Guid id,
        string name,
        DateTimeZone timeZone,
        IEnumerable<WeeklyMarketSessionSchedule> weeklySessions,
        IEnumerable<MarketCalendarDateOverride> dateOverrides)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("A market calendar ID must not be empty.", nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(timeZone);
        ArgumentNullException.ThrowIfNull(weeklySessions);
        ArgumentNullException.ThrowIfNull(dateOverrides);

        Id = id;
        Name = name;
        TimeZone = timeZone;
        _weeklySessions = weeklySessions
            .GroupBy(session => session.DayOfWeek)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<MarketSessionSchedule>)Array.AsReadOnly(
                    group.Select(session => session.Schedule).ToArray()));
        _dateOverrides = dateOverrides.ToDictionary(
            dateOverride => dateOverride.Date,
            dateOverride => (IReadOnlyList<MarketSessionSchedule>)Array.AsReadOnly(
                dateOverride.Sessions.ToArray()));
    }

    public Guid Id { get; }

    public string Name { get; }

    public DateTimeZone TimeZone { get; }

    public bool IsTradingDay(LocalDate tradingDate) => GetSchedules(tradingDate).Count > 0;

    public bool IsOpen(Instant instant) => GetSession(instant) is not null;

    public MarketSession? GetSession(Instant instant)
    {
        var localDate = instant.InZone(TimeZone).Date;

        foreach (var session in GetSessions(localDate.PlusDays(-1)))
        {
            if (session.Contains(instant))
            {
                return session;
            }
        }

        foreach (var session in GetSessions(localDate))
        {
            if (session.Contains(instant))
            {
                return session;
            }
        }

        return null;
    }

    public IReadOnlyList<MarketSession> GetSessions(LocalDate tradingDate)
    {
        var sessions = GetSchedules(tradingDate)
            .Select(schedule => CreateSession(tradingDate, schedule))
            .OrderBy(session => session.OpensAt)
            .ToArray();

        return Array.AsReadOnly(sessions);
    }

    private IReadOnlyList<MarketSessionSchedule> GetSchedules(LocalDate tradingDate)
    {
        if (_dateOverrides.TryGetValue(tradingDate, out var dateOverride))
        {
            return dateOverride;
        }

        return _weeklySessions.TryGetValue(tradingDate.DayOfWeek, out var weeklySessions)
            ? weeklySessions
            : [];
    }

    private MarketSession CreateSession(LocalDate tradingDate, MarketSessionSchedule schedule)
    {
        var closesOn = schedule.ClosesAt > schedule.OpensAt
            ? tradingDate
            : tradingDate.PlusDays(1);
        var opensAt = TimeZone.AtLeniently(tradingDate.At(schedule.OpensAt)).ToInstant();
        var closesAt = TimeZone.AtLeniently(closesOn.At(schedule.ClosesAt)).ToInstant();

        return new MarketSession(tradingDate, opensAt, closesAt, schedule.Type);
    }
}

internal sealed record MarketSessionSchedule(
    LocalTime OpensAt,
    LocalTime ClosesAt,
    MarketSessionType Type);

internal sealed record WeeklyMarketSessionSchedule(
    IsoDayOfWeek DayOfWeek,
    MarketSessionSchedule Schedule);

internal sealed record MarketCalendarDateOverride(
    LocalDate Date,
    IReadOnlyList<MarketSessionSchedule> Sessions);
