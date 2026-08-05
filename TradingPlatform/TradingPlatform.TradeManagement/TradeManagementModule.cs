using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.TradeManagement;

public static class TradeManagementModule
{
    public static IServiceCollection AddTradeManagementModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
