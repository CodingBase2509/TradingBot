using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.Instruments;

public sealed record Instrument
{
    internal Instrument(
        InstrumentId id,
        string name,
        InstrumentType type,
        string exchange,
        string currency,
        decimal tickSize,
        decimal tickValue,
        decimal minimumQuantity,
        InstrumentCapabilities capabilities,
        IReadOnlyList<ProviderSymbol> providerSymbols,
        Guid calendarId,
        Guid? rolloverRuleId = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(exchange);
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);

        if (type is InstrumentType.Unknown || !Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "The instrument type must be known.");
        }

        if (tickSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tickSize), tickSize, "Tick size must be greater than zero.");
        }

        if (tickValue <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tickValue), tickValue, "Tick value must be greater than zero.");
        }

        if (minimumQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumQuantity),
                minimumQuantity,
                "Minimum quantity must be greater than zero.");
        }

        if (!capabilities.SupportsLong && !capabilities.SupportsShort)
        {
            throw new ArgumentException("An instrument must support at least one trade direction.", nameof(capabilities));
        }

        if (capabilities.RequiresRollover && !capabilities.HasExpiringContracts)
        {
            throw new ArgumentException(
                "Only instruments with expiring contracts can require rollover.",
                nameof(capabilities));
        }

        ArgumentNullException.ThrowIfNull(providerSymbols);
        if (providerSymbols.Count == 0 || !providerSymbols.Any(IsValidMarketDataSymbol))
        {
            throw new ArgumentException("At least one valid market data symbol is required.", nameof(providerSymbols));
        }

        if (providerSymbols.Any(IsInvalidProviderSymbol) || HasDuplicateProviders(providerSymbols))
        {
            throw new ArgumentException("Provider symbols must be valid and unique per provider kind.", nameof(providerSymbols));
        }

        if (calendarId == Guid.Empty)
        {
            throw new ArgumentException("A calendar must be referenced.", nameof(calendarId));
        }

        if (capabilities.RequiresRollover && (!rolloverRuleId.HasValue || rolloverRuleId.Value == Guid.Empty))
        {
            throw new ArgumentException(
                "A non-empty rollover rule must be specified when the instrument requires rollover.",
                nameof(rolloverRuleId));
        }

        if (!capabilities.RequiresRollover && rolloverRuleId is not null)
        {
            throw new ArgumentException(
                "A rollover rule must not be specified when rollover is disabled.",
                nameof(rolloverRuleId));
        }

        Id = id;
        Name = name;
        Type = type;
        Exchange = exchange;
        Currency = currency;
        TickSize = tickSize;
        TickValue = tickValue;
        MinimumQuantity = minimumQuantity;
        Capabilities = capabilities;
        ProviderSymbols = providerSymbols.ToArray();
        CalendarId = calendarId;
        RolloverRuleId = rolloverRuleId;
    }

    public InstrumentId Id { get; }

    public string Name { get; }

    public InstrumentType Type { get; }

    public string Exchange { get; }

    public string Currency { get; }

    public decimal TickSize { get; }

    public decimal TickValue { get; }

    public decimal MinimumQuantity { get; }

    public InstrumentCapabilities Capabilities { get; }

    public IReadOnlyList<ProviderSymbol> ProviderSymbols { get; }

    public Guid CalendarId { get; }

    public Guid? RolloverRuleId { get; }

    private static bool IsValidMarketDataSymbol(ProviderSymbol providerSymbol) =>
        providerSymbol is not null &&
        providerSymbol.Kind is ProviderKind.MarketData &&
        !string.IsNullOrWhiteSpace(providerSymbol.Provider) &&
        !string.IsNullOrWhiteSpace(providerSymbol.Symbol);

    private static bool IsInvalidProviderSymbol(ProviderSymbol providerSymbol) =>
        providerSymbol is null ||
        providerSymbol.Kind is ProviderKind.Unknown ||
        !Enum.IsDefined(providerSymbol.Kind) ||
        string.IsNullOrWhiteSpace(providerSymbol.Provider) ||
        string.IsNullOrWhiteSpace(providerSymbol.Symbol);

    private static bool HasDuplicateProviders(IEnumerable<ProviderSymbol> providerSymbols)
    {
        var providerKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return providerSymbols.Any(providerSymbol =>
            !providerKeys.Add($"{(int)providerSymbol.Kind}:{providerSymbol.Provider}"));
    }
}
