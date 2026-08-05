namespace TradingPlatform.Market.Instruments.Providers;

public sealed record ProviderSymbol(
    ProviderKind Kind,
    string Provider,
    string Symbol);
