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
                var apiConfig = sp.GetRequiredService<IOptions<Config.MediaWikiApiConfig>>().Value;
                var fullApiUrl = apiConfig.GetFullApiUrl();
                if (!string.IsNullOrEmpty(fullApiUrl))
                    client.BaseAddress = new Uri(fullApiUrl);
            });
        return services.BuildServiceProvider();
    }
}
