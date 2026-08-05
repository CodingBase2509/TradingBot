using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.Reconciliation;

public static class ReconciliationModule
{
    public static IServiceCollection AddReconciliationModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
