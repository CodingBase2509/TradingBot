using TradingPlatform.Market.Instruments;
using TradingPlatform.Market.Instruments.Config;
using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Config;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Tests.Builders;

internal sealed class InstrumentConfigBuilder
{
    private const string ContentHash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    private ConfigVersionMetadata? _metadata;
    private InstrumentId? _instrumentId = new("MES");
    private string _name = "Micro E-mini S&P 500 Future";
    private InstrumentType _instrumentType = InstrumentType.Future;
    private string _exchange = "CME";
    private string _currency = "USD";
    private decimal _tickSize = 0.25m;
    private decimal _tickValue = 1.25m;
    private decimal _minimumQuantity = 1m;
    private InstrumentCapabilities _capabilities = new(
        SupportsLong: true,
        SupportsShort: true,
        HasExpiringContracts: true,
        RequiresRollover: true);
    private IReadOnlyList<ProviderSymbol>? _providerSymbols =
    [
        new ProviderSymbol(ProviderKind.MarketData, "Databento", "MES.FUT"),
        new ProviderSymbol(ProviderKind.Broker, "InteractiveBrokers", "MES"),
    ];
    private string? _calendarId = TestInstruments.MesCalendarId.ToString();
    private string? _rolloverRuleId = TestInstruments.MesRolloverRuleId.ToString();

    public InstrumentConfigBuilder WithStatus(ConfigStatus status)
    {
        _metadata = CreateMetadata(status);
        return this;
    }

    public InstrumentConfigBuilder WithMetadata(ConfigVersionMetadata? metadata)
    {
        _metadata = metadata;
        return this;
    }

    public InstrumentConfigBuilder WithInstrumentId(InstrumentId? instrumentId)
    {
        _instrumentId = instrumentId;
        return this;
    }

    public InstrumentConfigBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public InstrumentConfigBuilder WithInstrumentType(InstrumentType instrumentType)
    {
        _instrumentType = instrumentType;
        return this;
    }

    public InstrumentConfigBuilder WithExchange(string exchange)
    {
        _exchange = exchange;
        return this;
    }

    public InstrumentConfigBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public InstrumentConfigBuilder WithTradingValues(decimal tickSize, decimal tickValue, decimal minimumQuantity)
    {
        _tickSize = tickSize;
        _tickValue = tickValue;
        _minimumQuantity = minimumQuantity;
        return this;
    }

    public InstrumentConfigBuilder WithCapabilities(InstrumentCapabilities capabilities)
    {
        _capabilities = capabilities;
        return this;
    }

    public InstrumentConfigBuilder WithProviderSymbols(IReadOnlyList<ProviderSymbol>? providerSymbols)
    {
        _providerSymbols = providerSymbols;
        return this;
    }

    public InstrumentConfigBuilder WithCalendarId(string? calendarId)
    {
        _calendarId = calendarId;
        return this;
    }

    public InstrumentConfigBuilder WithRolloverRuleId(string? rolloverRuleId)
    {
        _rolloverRuleId = rolloverRuleId;
        return this;
    }

    public InstrumentConfig Build() => new()
    {
        Metadata = _metadata ?? CreateMetadata(ConfigStatus.Active),
        InstrumentId = _instrumentId!,
        Name = _name,
        InstrumentType = _instrumentType,
        Exchange = _exchange,
        Currency = _currency,
        TickSize = _tickSize,
        TickValue = _tickValue,
        MinimumQuantity = _minimumQuantity,
        Capabilities = _capabilities,
        ProviderSymbols = _providerSymbols!,
        CalendarId = _calendarId,
        RolloverRuleId = _rolloverRuleId,
    };

    private static ConfigVersionMetadata CreateMetadata(ConfigStatus status) => new(
        ConfigId.Create(),
        version: 1,
        schemaVersion: 1,
        status,
        new DateTimeOffset(2026, 8, 5, 12, 0, 0, TimeSpan.Zero),
        "user:test",
        "Test config",
        new ConfigContentHash(ContentHash));
}
