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
    private const bool IS_BASE_APPSETTINGS_OPTIONAL = false;
    private const bool RELOAD_BASE_APPSETTINGS_OM_CHANE = false;
    public const string BASE_APPSETTINGS_FILE_PATH = "appsettings.default.json";

    public static IServiceCollection ConfigurePlaywright(this IServiceCollection services, IConfiguration config)
    {
        var playwrightBrowserConfig = config.GetSection(nameof(PlaywrightBrowserConfig)).Get<PlaywrightBrowserConfig>();
        var playwrightBrowserContextConfig = config.GetSection(nameof(PlaywrightBrowserContextConfig)).Get<PlaywrightBrowserContextConfig>();

        services.AddSingleton(_ => Playwright.CreateAsync().GetAwaiter().GetResult());
        services.AddSingleton(provider => provider.GetRequiredService<IPlaywright>()
            [playwrightBrowserConfig!.BrowserType].LaunchAsync(playwrightBrowserConfig).GetAwaiter().GetResult());
        services.AddSingleton(provider =>
            provider.GetRequiredService<IBrowser>().NewContextAsync(playwrightBrowserContextConfig!).GetAwaiter()
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
        // Register the shared Wikipedia base URL
        services.Configure<WikipediaConfig>(config.GetSection("Wikipedia"));
        
        // Register config sections
        services.Configure<PageNavigatorConfig>(config.GetSection(nameof(PageNavigatorConfig)));
        services.Configure<MediaWikiApiConfig>(config.GetSection(nameof(MediaWikiApiConfig)));
        services.Configure<PlaywrightBrowserConfig>(config.GetSection(nameof(PlaywrightBrowserConfig)));
        services.Configure<PlaywrightBrowserContextConfig>(config.GetSection(nameof(PlaywrightBrowserContextConfig)));
        
        // Post-configure to inject base URL from WikipediaConfig
        services.AddSingleton<IPostConfigureOptions<PageNavigatorConfig>, WikipediaOptionsPostConfigure>();
        services.AddSingleton<IPostConfigureOptions<MediaWikiApiConfig>, WikipediaOptionsPostConfigure>();
        
        return services;
    }

    public static IConfigurationBuilder AddDefaultAppsettings(this IConfigurationBuilder builder,
        bool isBaseAppsettingsOptional = IS_BASE_APPSETTINGS_OPTIONAL,
        bool reloadBaseAppsettingsOnChange = RELOAD_BASE_APPSETTINGS_OM_CHANE)
    {
        var sdkFileProvider = new EmbeddedFileProvider(typeof(ConfigurationExtensions).Assembly);
        builder.AddJsonFile(sdkFileProvider, BASE_APPSETTINGS_FILE_PATH, isBaseAppsettingsOptional,
            reloadBaseAppsettingsOnChange);

        return builder;
    }
}