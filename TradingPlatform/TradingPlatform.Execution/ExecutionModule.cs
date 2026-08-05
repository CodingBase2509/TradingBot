using Microsoft.Extensions.DependencyInjection;

namespace TradingPlatform.Execution;

public static class ExecutionModule
{
    public static IServiceCollection AddExecutionModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
