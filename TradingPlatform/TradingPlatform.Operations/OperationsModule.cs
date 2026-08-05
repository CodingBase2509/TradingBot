using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.Operations;

public static class OperationsModule
{
    public static IServiceCollection AddOperationsModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
