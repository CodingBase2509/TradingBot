using NodaTime;
using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Calendars.Config;
using TradingPlatform.Platform.Config;

namespace TradingPlatform.Tests.Builders;

internal sealed class MarketCalendarConfigBuilder
{
    private const string ContentHash = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

    private ConfigVersionMetadata? _metadata = CreateMetadata(ConfigStatus.Active);
    private string _calendarId = TestMarketCalendars.NyseId.ToString();
    private string _name = "New York Stock Exchange";
    private string _timeZoneId = "America/New_York";
    private IReadOnlyList<MarketSessionConfig>? _weeklySessions = CreateDefaultWeeklySessions();
    private IReadOnlyList<MarketCalendarDateOverrideConfig>? _dateOverrides = [];

    public MarketCalendarConfigBuilder WithStatus(ConfigStatus status)
    {
        _metadata = CreateMetadata(status);
        return this;
    }

    public MarketCalendarConfigBuilder WithMetadata(ConfigVersionMetadata? metadata)
    {
        _metadata = metadata;
        return this;
    }

    public MarketCalendarConfigBuilder WithCalendarId(string calendarId)
    {
        _calendarId = calendarId;
        return this;
    }

    public MarketCalendarConfigBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public MarketCalendarConfigBuilder WithTimeZoneId(string timeZoneId)
    {
        _timeZoneId = timeZoneId;
        return this;
    }

    public MarketCalendarConfigBuilder WithWeeklySessions(IReadOnlyList<MarketSessionConfig>? weeklySessions)
    {
        _weeklySessions = weeklySessions;
        return this;
    }

    public MarketCalendarConfigBuilder WithDateOverrides(
        IReadOnlyList<MarketCalendarDateOverrideConfig>? dateOverrides)
    {
        _dateOverrides = dateOverrides;
        return this;
    }

    public MarketCalendarConfig Build() => new()
    {
        Metadata = _metadata!,
        CalendarId = _calendarId,
        Name = _name,
        TimeZoneId = _timeZoneId,
        WeeklySessions = _weeklySessions!,
        DateOverrides = _dateOverrides!,
    };

    private static MarketSessionConfig[] CreateDefaultWeeklySessions() =>
        Enumerable.Range((int)IsoDayOfWeek.Monday, 5)
            .Select(day => new MarketSessionConfig(
                (IsoDayOfWeek)day,
                new LocalTime(9, 30),
                new LocalTime(16, 0),
                MarketSessionType.Regular))
            .ToArray();

    private static ConfigVersionMetadata CreateMetadata(ConfigStatus status) => new(
        ConfigId.Create(),
        version: 1,
        schemaVersion: 1,
        status,
        new DateTimeOffset(2026, 8, 5, 12, 0, 0, TimeSpan.Zero),
        "user:test",
        "Test calendar config",
        new ConfigContentHash(ContentHash));
}
