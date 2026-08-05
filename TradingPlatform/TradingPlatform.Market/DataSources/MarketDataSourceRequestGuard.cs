namespace TradingPlatform.Market.DataSources;

internal static class MarketDataSourceRequestGuard
{
    public static IReadOnlyList<string> ValidateAndCopySymbols(IEnumerable<string> symbols)
    {
        ArgumentNullException.ThrowIfNull(symbols);

        var snapshot = symbols.ToArray();
        if (snapshot.Length == 0)
        {
            throw new ArgumentException("At least one provider symbol is required.", nameof(symbols));
        }

        if (snapshot.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Provider symbols must not be empty.", nameof(symbols));
        }

        if (snapshot.Any(symbol => !string.Equals(symbol, symbol.Trim(), StringComparison.Ordinal)))
        {
            throw new ArgumentException(
                "Provider symbols must not contain leading or trailing whitespace.",
                nameof(symbols));
        }

        if (snapshot.Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
        {
            throw new ArgumentException("Provider symbols must be unique.", nameof(symbols));
        }

        return Array.AsReadOnly(snapshot);
    }
}
