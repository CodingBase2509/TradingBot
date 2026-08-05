using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.FeatureIntelligence;

public static class FeatureIntelligenceModule
{
    public static IServiceCollection AddFeatureIntelligenceModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
