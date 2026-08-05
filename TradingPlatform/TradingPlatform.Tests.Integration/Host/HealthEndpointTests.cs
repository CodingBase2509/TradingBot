using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TradingPlatform.Tests.Integration.Host;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _application;

    public HealthEndpointTests(WebApplicationFactory<Program> application)
    {
        _application = application;
    }

    [Fact]
    public async Task HealthEndpointReportsAHealthyHost()
    {
        using var client = _application.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", content);
    }
}
