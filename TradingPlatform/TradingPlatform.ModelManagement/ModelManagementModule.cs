using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.ModelManagement;

public static class ModelManagementModule
{
    public static IServiceCollection AddModelManagementModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
