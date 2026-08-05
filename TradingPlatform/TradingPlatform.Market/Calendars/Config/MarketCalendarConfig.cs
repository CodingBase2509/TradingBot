using NodaTime;
using TradingPlatform.Platform.Config;

namespace TradingPlatform.Market.Calendars.Config;

internal sealed record MarketCalendarConfig
{
    public required ConfigVersionMetadata Metadata { get; init; }

    public required string CalendarId { get; init; }

    public required string Name { get; init; }

    public required string TimeZoneId { get; init; }

    public required IReadOnlyList<MarketSessionConfig> WeeklySessions { get; init; }

    public required IReadOnlyList<MarketCalendarDateOverrideConfig> DateOverrides { get; init; }
}

internal sealed record MarketSessionConfig(
    IsoDayOfWeek DayOfWeek,
    LocalTime OpensAt,
    LocalTime ClosesAt,
    MarketSessionType Type);

internal sealed record MarketSessionTimeConfig(
    LocalTime OpensAt,
    LocalTime ClosesAt,
    MarketSessionType Type);

internal sealed record MarketCalendarDateOverrideConfig(
    LocalDate Date,
    IReadOnlyList<MarketSessionTimeConfig> Sessions);
