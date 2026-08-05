namespace TradingPlatform.Market.Instruments;

public readonly record struct InstrumentCapabilities(
    bool SupportsLong,
    bool SupportsShort,
    bool HasExpiringContracts,
    bool RequiresRollover);
