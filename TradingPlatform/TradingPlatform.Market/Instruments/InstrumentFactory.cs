using TradingPlatform.Market.Instruments.Config;
using TradingPlatform.Platform.Config;

namespace TradingPlatform.Market.Instruments;

internal static class InstrumentFactory
{
    public static Instrument Create(InstrumentConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var validationResult = InstrumentConfigValidator.Validate(config);
        if (!validationResult.IsValid)
        {
            var issueCodes = string.Join(", ", validationResult.Issues.Select(issue => issue.Code));
            throw new ArgumentException($"The instrument config is invalid: {issueCodes}.", nameof(config));
        }

        if (config.Metadata.Status is not ConfigStatus.Active)
        {
            throw new InvalidOperationException("Only an active instrument config can create a runtime instrument.");
        }

        var calendarId = Guid.Parse(config.CalendarId!);
        Guid? rolloverRuleId = string.IsNullOrWhiteSpace(config.RolloverRuleId)
            ? null
            : Guid.Parse(config.RolloverRuleId);

        return new Instrument(
            config.InstrumentId,
            config.Name,
            config.InstrumentType,
            config.Exchange,
            config.Currency,
            config.TickSize,
            config.TickValue,
            config.MinimumQuantity,
            config.Capabilities,
            config.ProviderSymbols,
            calendarId,
            rolloverRuleId);
    }
}
