using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Config;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.Instruments.Config;

internal sealed record InstrumentConfig
{
    public required ConfigVersionMetadata Metadata { get; init; }
    public required InstrumentId InstrumentId { get; init; }
    public required string Name { get; init; }
    public required InstrumentType InstrumentType { get; init; }
    public required string Exchange { get; init; }
    public required string Currency { get; init; }

    public required decimal TickSize { get; init; }
    public required decimal TickValue { get; init; }
    public required decimal MinimumQuantity { get; init; }

    public required InstrumentCapabilities Capabilities { get; init; }
    public required IReadOnlyList<ProviderSymbol> ProviderSymbols { get; init; }

    public string? CalendarId { get; init; }
    public string? RolloverRuleId { get; init; }
}
