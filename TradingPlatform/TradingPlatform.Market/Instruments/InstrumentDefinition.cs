using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.Instruments;

public sealed record InstrumentDefinition
{
    public InstrumentDefinition(
        InstrumentId id,
        string name,
        InstrumentType type,
        string exchange,
        string currency,
        decimal tickSize,
        decimal tickValue,
        decimal minimumQuantity,
        InstrumentCapabilities capabilities)
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

        Id = id;
        Name = name;
        Type = type;
        Exchange = exchange;
        Currency = currency;
        TickSize = tickSize;
        TickValue = tickValue;
        MinimumQuantity = minimumQuantity;
        Capabilities = capabilities;
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
}
