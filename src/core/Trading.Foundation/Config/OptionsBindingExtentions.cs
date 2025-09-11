using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Trading.Foundation.Config;

/// <summary>
/// Helper methods to bind and validate options using the built-in Options pattern.
/// </summary>
public static class OptionsBindingExtentions
{
    /// <summary>
    /// Binds <typeparamref name="TOptions"/> to the configuration root, enables data annotation validation,
    /// and validates on startup.
    /// </summary>
    /// <typeparam name="TOptions">Options POCO type.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration root.</param>
    /// <returns>An <see cref="OptionsBuilder{TOptions}"/> for further customization.</returns>
    public static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, new()
    {
        return services
            .AddOptions<TOptions>()
            .Bind(configuration)
            .ValidateOnStart();
    }

    /// <summary>
    /// Binds <typeparamref name="TOptions"/> to a specific configuration section, enables data annotation validation,
    /// and validates on startup.
    /// </summary>
    /// <typeparam name="TOptions">Options POCO type.</typeparam>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration root.</param>
    /// <param name="sectionName">Configuration section name to bind from.</param>
    /// <returns>An <see cref="OptionsBuilder{TOptions}"/> for further customization.</returns>
    public static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string sectionName)
        where TOptions : class, new()
    {
        var section = configuration.GetSection(sectionName);
        return services
            .AddOptions<TOptions>()
            .Bind(section)
            .ValidateOnStart();
    }
}