using Microsoft.Extensions.Options;
using Trading.Core.Config.Live;

namespace Trading.Foundation.Config;

/// <summary>
/// Lightweight snapshot of dynamic decision parameters derived from <see cref="LiveOptions"/>.
/// Uses <see cref="IOptionsMonitor{TOptions}"/> to update a cached struct without hot-path allocations.
/// </summary>
public sealed class DecisionConfigProvider : IDisposable
{
    private DecisionConfig current;
    private readonly IDisposable? subscription;

    /// <summary>Creates the provider and subscribes to configuration changes.</summary>
    /// <param name="optionsMonitor">Options monitor bound to live configuration.</param>
    public DecisionConfigProvider(IOptionsMonitor<LiveOptions> optionsMonitor)
    {
        this.current = Map(optionsMonitor.CurrentValue);
        this.subscription = optionsMonitor?.OnChange(o => Volatile.Write(ref current, Map(o)));
    }

    /// <summary>Gets the current snapshot (cheap volatile read).</summary>
    public DecisionConfig Current => Volatile.Read(ref current);

    /// <summary>Disposes the underlying change subscription.</summary>
    public void Dispose() => subscription?.Dispose();

    private static DecisionConfig Map(LiveOptions o) => new(
        ThresholdLong:        o.Model?.Long?.Threshold   ?? 0.60,
        ThresholdShort:       o.Model?.Short?.Threshold  ?? 0.58,
        MinimumRiskReward:    o.Risk?.RrMinCosted        ?? 1.5,
        MaximumCostRatio:     o.Risk?.CostRatioMax       ?? 0.15,
        RiskFractionPerTrade: o.Risk?.RiskPct            ?? 0.0075
    );
}