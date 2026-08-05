using TradingPlatform.Market.Instruments;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Builders;

public static class TestInstruments
{
    public static InstrumentDefinition Mes() => new(
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
            RequiresRollover: true));
}
