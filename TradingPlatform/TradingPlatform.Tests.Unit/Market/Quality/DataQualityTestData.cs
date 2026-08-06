using NodaTime;
using TradingPlatform.Market.Calendars;
using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.MarketData;
using TradingPlatform.Market.Quality;
using TradingPlatform.Platform.Identifiers;
using TradingPlatform.Tests.Builders;

namespace TradingPlatform.Tests.Unit.Market.Quality;

internal static class DataQualityTestData
{
    public static Instant DefaultOccurredAt { get; } = Instant.FromUtc(2026, 8, 3, 14, 0);

    public static Instrument Instrument() => TestInstruments.Mes();

    public static MarketCalendar Calendar() => MarketCalendarFactory.Create(
        new MarketCalendarConfigBuilder()
            .WithCalendarId(TestInstruments.MesCalendarId.ToString())
            .Build());

    public static DataQualityValidator HistoricalValidator() => new(
        DataQualityContext.CreateHistorical(Instrument(), Calendar()),
        new FixedClock(DefaultOccurredAt));

    public static DataQualityValidator LiveValidator(
        Instant? now = null,
        Duration? maxDataAge = null,
        Duration? maxFutureSkew = null) =>
        new(
            DataQualityContext.CreateLive(
                Instrument(),
                Calendar(),
                maxDataAge ?? Duration.FromMinutes(5),
                maxFutureSkew ?? Duration.FromMinutes(1)),
            new FixedClock(now ?? DefaultOccurredAt));

    public static Trade Trade(
        InstrumentId? instrumentId = null,
        Instant? occurredAt = null,
        decimal price = 5280.25m,
        decimal quantity = 1m,
        long? sequenceNumber = 10) =>
        new(
            instrumentId ?? new InstrumentId("MES"),
            occurredAt ?? DefaultOccurredAt,
            price,
            quantity,
            new MarketDataOrigin("Databento", sequenceNumber));

    public static Quote Quote(
        InstrumentId? instrumentId = null,
        Instant? occurredAt = null,
        decimal bidPrice = 5280.00m,
        decimal bidQuantity = 1m,
        decimal askPrice = 5280.25m,
        decimal askQuantity = 1m,
        long? sequenceNumber = 10) =>
        new(
            instrumentId ?? new InstrumentId("MES"),
            occurredAt ?? DefaultOccurredAt,
            bidPrice,
            bidQuantity,
            askPrice,
            askQuantity,
            new MarketDataOrigin("Databento", sequenceNumber));

    private sealed class FixedClock(Instant currentInstant) : IClock
    {
        public Instant GetCurrentInstant() => currentInstant;
    }
}
