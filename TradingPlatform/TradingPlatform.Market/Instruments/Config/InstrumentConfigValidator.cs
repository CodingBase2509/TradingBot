using TradingPlatform.Market.Instruments.Providers;

namespace TradingPlatform.Market.Instruments.Config;

internal static class InstrumentConfigValidator
{
    public static InstrumentConfigValidationResult Validate(InstrumentConfig instrumentConfig)
    {
        ArgumentNullException.ThrowIfNull(instrumentConfig);

        var issues = new List<InstrumentConfigIssue>();

        if (instrumentConfig.Metadata is null)
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingMetadata, "Config metadata is required.");
        }

        if (instrumentConfig.InstrumentId is null)
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingInstrumentId, "An instrument ID is required.");
        }

        if (string.IsNullOrWhiteSpace(instrumentConfig.Name))
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingName, "An instrument name is required.");
        }

        if (instrumentConfig.InstrumentType is InstrumentType.Unknown || !Enum.IsDefined(instrumentConfig.InstrumentType))
        {
            AddIssue(issues, InstrumentConfigIssueCode.UnknownInstrumentType, "The instrument type must be known.");
        }

        if (string.IsNullOrWhiteSpace(instrumentConfig.Exchange))
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingExchange, "An exchange is required.");
        }

        if (string.IsNullOrWhiteSpace(instrumentConfig.Currency))
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingCurrency, "A currency is required.");
        }

        if (instrumentConfig.TickSize <= 0)
        {
            AddIssue(issues, InstrumentConfigIssueCode.InvalidTickSize, "Tick size must be greater than zero.");
        }

        if (instrumentConfig.TickValue <= 0)
        {
            AddIssue(issues, InstrumentConfigIssueCode.InvalidTickValue, "Tick value must be greater than zero.");
        }

        if (instrumentConfig.MinimumQuantity <= 0)
        {
            AddIssue(issues, InstrumentConfigIssueCode.InvalidMinimumQuantity, "Minimum quantity must be greater than zero.");
        }

        if (!instrumentConfig.Capabilities.SupportsLong && !instrumentConfig.Capabilities.SupportsShort)
        {
            AddIssue(
                issues,
                InstrumentConfigIssueCode.UnsupportedTradeDirection,
                "An instrument must support at least one trade direction.");
        }

        ValidateProviderSymbols(instrumentConfig.ProviderSymbols, issues);
        ValidateCalendar(instrumentConfig.CalendarId, issues);
        ValidateRollover(instrumentConfig, issues);

        return new(issues.ToArray());
    }

    private static void ValidateProviderSymbols(
        IReadOnlyList<ProviderSymbol>? providerSymbols,
        ICollection<InstrumentConfigIssue> issues)
    {
        if (providerSymbols is null || providerSymbols.Count == 0)
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingProviderSymbols, "At least one provider symbol is required.");
            AddIssue(issues, InstrumentConfigIssueCode.MissingMarketDataSymbol, "At least one market data symbol is required.");
            return;
        }

        var providerKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var hasValidMarketDataSymbol = false;

        foreach (var providerSymbol in providerSymbols)
        {
            if (providerSymbol is null)
            {
                AddIssue(issues, InstrumentConfigIssueCode.MissingProviderSymbol, "Provider symbols must not contain null entries.");
                continue;
            }

            var kindIsValid = providerSymbol.Kind is not ProviderKind.Unknown && Enum.IsDefined(providerSymbol.Kind);
            var providerIsValid = !string.IsNullOrWhiteSpace(providerSymbol.Provider);
            var symbolIsValid = !string.IsNullOrWhiteSpace(providerSymbol.Symbol);

            if (!kindIsValid)
            {
                AddIssue(issues, InstrumentConfigIssueCode.UnknownProviderKind, "The provider kind must be known.");
            }

            if (!providerIsValid)
            {
                AddIssue(issues, InstrumentConfigIssueCode.MissingProviderName, "A provider name is required.");
            }

            if (!symbolIsValid)
            {
                AddIssue(issues, InstrumentConfigIssueCode.MissingProviderSymbol, "A provider symbol is required.");
            }

            if (kindIsValid && providerIsValid)
            {
                var providerKey = $"{(int)providerSymbol.Kind}:{providerSymbol.Provider}";
                if (!providerKeys.Add(providerKey))
                {
                    AddIssue(
                        issues,
                        InstrumentConfigIssueCode.DuplicateProviderSymbol,
                        $"Provider '{providerSymbol.Provider}' is configured more than once for kind '{providerSymbol.Kind}'.");
                }
            }

            hasValidMarketDataSymbol |= providerSymbol.Kind is ProviderKind.MarketData && providerIsValid && symbolIsValid;
        }

        if (!hasValidMarketDataSymbol)
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingMarketDataSymbol, "At least one valid market data symbol is required.");
        }
    }

    private static void ValidateCalendar(string? calendarId, ICollection<InstrumentConfigIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(calendarId))
        {
            AddIssue(issues, InstrumentConfigIssueCode.MissingCalendar, "A calendar ID is required.");
            return;
        }

        if (!Guid.TryParse(calendarId, out var parsedCalendarId) || parsedCalendarId == Guid.Empty)
        {
            AddIssue(issues, InstrumentConfigIssueCode.InvalidCalendarId, "The calendar ID must be a non-empty GUID.");
        }
    }

    private static void ValidateRollover(
        InstrumentConfig instrumentConfig,
        ICollection<InstrumentConfigIssue> issues)
    {
        if (instrumentConfig.Capabilities.RequiresRollover && !instrumentConfig.Capabilities.HasExpiringContracts)
        {
            AddIssue(
                issues,
                InstrumentConfigIssueCode.RolloverWithoutExpiringContracts,
                "Only instruments with expiring contracts can require rollover.");
        }

        if (instrumentConfig.Capabilities.RequiresRollover)
        {
            if (string.IsNullOrWhiteSpace(instrumentConfig.RolloverRuleId))
            {
                AddIssue(issues, InstrumentConfigIssueCode.MissingRolloverRule, "A rollover rule ID is required.");
            }
            else if (!Guid.TryParse(instrumentConfig.RolloverRuleId, out var rolloverRuleId) || rolloverRuleId == Guid.Empty)
            {
                AddIssue(
                    issues,
                    InstrumentConfigIssueCode.InvalidRolloverRuleId,
                    "The rollover rule ID must be a non-empty GUID.");
            }

            return;
        }

        if (!string.IsNullOrWhiteSpace(instrumentConfig.RolloverRuleId))
        {
            AddIssue(
                issues,
                InstrumentConfigIssueCode.RolloverRuleWithoutRollover,
                "A rollover rule must not be configured when rollover is disabled.");
        }
    }

    private static void AddIssue(
        ICollection<InstrumentConfigIssue> issues,
        InstrumentConfigIssueCode code,
        string message)
    {
        issues.Add(new InstrumentConfigIssue(code, message));
    }
}
