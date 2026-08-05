using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Builders;

public static class TestInstruments
{
    public static Guid MesCalendarId { get; } = Guid.Parse("11111111-1111-7111-8111-111111111111");

    public static Guid MesRolloverRuleId { get; } = Guid.Parse("22222222-2222-7222-8222-222222222222");

    public static Instrument Mes() => new(
        new InstrumentId("MES"),
        "Micro E-mini S&P 500 Future",
        InstrumentType.Future,
        "CME",
        "USD",
        tickSize: 0.25m,
        tickValue: 1.25m,
        minimumQuantity: 1m,
        new InstrumentCapabilities(
            SupportsLong: true,
            SupportsShort: true,
            HasExpiringContracts: true,
            RequiresRollover: true),
        [
            new ProviderSymbol(ProviderKind.MarketData, "Databento", "MES.FUT"),
            new ProviderSymbol(ProviderKind.Broker, "InteractiveBrokers", "MES"),
        ],
        MesCalendarId,
        MesRolloverRuleId);
}
