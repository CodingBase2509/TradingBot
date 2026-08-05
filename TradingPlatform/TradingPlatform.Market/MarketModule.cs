using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.Market;

public static class MarketModule
{
    public static IServiceCollection AddMarketModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
