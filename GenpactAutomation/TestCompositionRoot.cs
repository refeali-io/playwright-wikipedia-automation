using GenpactAutomation.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GenpactAutomation;

/// <summary>
/// Builds the DI container for tests (config + services). Replaces Startup + TestServer.
/// </summary>
public static class TestCompositionRoot
{
    public static IServiceProvider BuildServiceProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton<Navigation.PageNavigator>();
        services.ConfigurePlaywright(configuration);
        services.AddConfiguration(configuration);
        services.AddPages();
        services.AddHttpClient<Api.IMediaWikiApiClient, Api.MediaWikiApiClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                var baseUrl = sp.GetRequiredService<IOptions<Config.PageNavigatorConfig>>().Value.BaseUrl;
                if (!string.IsNullOrEmpty(baseUrl))
                    client.BaseAddress = new Uri(baseUrl);
            });
        return services.BuildServiceProvider();
    }
}
