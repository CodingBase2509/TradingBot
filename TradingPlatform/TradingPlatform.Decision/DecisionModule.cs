using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.Decision;

public static class DecisionModule
{
    public static IServiceCollection AddDecisionModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
