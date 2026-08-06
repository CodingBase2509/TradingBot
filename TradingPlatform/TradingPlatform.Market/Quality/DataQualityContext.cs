using NodaTime;
using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Instruments;

namespace TradingPlatform.Market.Quality;

internal sealed record DataQualityContext
{
    private DataQualityContext(
        Instrument instrument,
        MarketCalendar calendar,
        DataQualityMode mode,
        Duration maxLiveDataAge,
        Duration maxFutureSkew)
    {
        ArgumentNullException.ThrowIfNull(instrument);
        ArgumentNullException.ThrowIfNull(calendar);

        if (instrument.CalendarId != calendar.Id)
        {
            throw new ArgumentException(
                "The market calendar does not belong to the instrument.",
                nameof(calendar));
        }

        Instrument = instrument;
        Calendar = calendar;
        Mode = mode;
        MaxLiveDataAge = maxLiveDataAge;
        MaxFutureSkew = maxFutureSkew;
    }

    public Instrument Instrument { get; }

    public MarketCalendar Calendar { get; }

    public DataQualityMode Mode { get; }

    public Duration MaxLiveDataAge { get; }

    public Duration MaxFutureSkew { get; }

    public static DataQualityContext CreateHistorical(
        Instrument instrument,
        MarketCalendar calendar) =>
        new(instrument, calendar, DataQualityMode.Historical, Duration.Zero, Duration.Zero);

    public static DataQualityContext CreateLive(
        Instrument instrument,
        MarketCalendar calendar,
        Duration maxLiveDataAge,
        Duration maxFutureSkew)
    {
        if (maxLiveDataAge <= Duration.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxLiveDataAge),
                maxLiveDataAge,
                "The maximum live data age must be greater than zero.");
        }

        if (maxFutureSkew < Duration.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxFutureSkew),
                maxFutureSkew,
                "The maximum future skew must not be negative.");
        }

        return new DataQualityContext(
            instrument,
            calendar,
            DataQualityMode.Live,
            maxLiveDataAge,
            maxFutureSkew);
    }
}
