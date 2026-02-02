using GenpactAutomation.Config;
using GenpactAutomation.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace GenpactAutomation.Extensions;

public static class ConfigurationExtensions
{
    private const bool IsBaseAppsettingsOptional = false;
    private const bool ReloadBaseAppsettingsOnChange = false;
    public const string BaseAppsettingsFilePath = "appsettings.default.json";

    public static IConfigurationBuilder AddDefaultAppsettings(this IConfigurationBuilder builder,
        bool isBaseAppsettingsOptional = IsBaseAppsettingsOptional,
        bool reloadBaseAppsettingsOnChange = ReloadBaseAppsettingsOnChange)
    {
        var embeddedProvider = new EmbeddedFileProvider(typeof(ConfigurationExtensions).Assembly);
        builder.AddJsonFile(embeddedProvider, BaseAppsettingsFilePath, isBaseAppsettingsOptional,
            reloadBaseAppsettingsOnChange);
        return builder;
    }

    public static IServiceCollection ConfigurePlaywright(this IServiceCollection services, IConfiguration config)
    {
        var playwrightBrowserConfig = config.GetSection(nameof(PlaywrightBrowserConfig)).Get<PlaywrightBrowserConfig>();
        var playwrightBrowserContextConfig = config.GetSection(nameof(PlaywrightBrowserContextConfig)).Get<PlaywrightBrowserContextConfig>();

        // Maximized window: use actual window size instead of default viewport
        playwrightBrowserContextConfig!.ViewportSize = ViewportSize.NoViewport;

        services.AddSingleton(_ => Playwright.CreateAsync().GetAwaiter().GetResult());
        services.AddSingleton(provider => provider.GetRequiredService<IPlaywright>()
            [playwrightBrowserConfig!.BrowserType].LaunchAsync(playwrightBrowserConfig).GetAwaiter().GetResult());
        services.AddSingleton(provider =>
            provider.GetRequiredService<IBrowser>().NewContextAsync(playwrightBrowserContextConfig).GetAwaiter()
                .GetResult());
        services.AddSingleton(provider =>
            provider.GetRequiredService<IBrowserContext>().NewPageAsync().GetAwaiter().GetResult());

        return services;
    }

    public static IServiceCollection AddPages(this IServiceCollection services)
    {
        services.AddSingleton<WikipediaPlaywrightPage>();
        services.AddSingleton<IEnumerable<Page>>(sp => new Page[]
        {
            sp.GetRequiredService<WikipediaPlaywrightPage>()
        });
        return services;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<WikipediaConfig>(config.GetSection("Wikipedia"));
        services.Configure<PageNavigatorConfig>(config.GetSection(nameof(PageNavigatorConfig)));
        services.Configure<MediaWikiApiConfig>(config.GetSection(nameof(MediaWikiApiConfig)));
        services.Configure<PlaywrightBrowserConfig>(config.GetSection(nameof(PlaywrightBrowserConfig)));
        services.Configure<PlaywrightBrowserContextConfig>(config.GetSection(nameof(PlaywrightBrowserContextConfig)));
        services.AddSingleton<IPostConfigureOptions<PageNavigatorConfig>, WikipediaOptionsPostConfigure>();
        services.AddSingleton<IPostConfigureOptions<MediaWikiApiConfig>, WikipediaOptionsPostConfigure>();
        return services;
    }
}
