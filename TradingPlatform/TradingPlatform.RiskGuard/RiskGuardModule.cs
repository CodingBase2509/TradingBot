using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.RiskGuard;

public static class RiskGuardModule
{
    public static IServiceCollection AddRiskGuardModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
