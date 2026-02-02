using System.Threading.Tasks;
using GenpactAutomation.Api;
using GenpactAutomation.Extensions;
using GenpactAutomation.Navigation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace GenpactAutomation;

public abstract class SdkPlaywrightBaseTest
{
    private IServiceProvider? _serviceProvider;
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext BrowserContext = null!;
    protected IPage Page = null!;
    protected PageNavigator PageNavigator = null!;
    protected IMediaWikiApiClient MediaWikiApiClient = null!;

    protected virtual string AppsettingsJsonFilePath => "appsettings.development.json";
    private const bool JsonFileIsOptional = true;

    protected virtual async Task BeforeTestAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddDefaultAppsettings()
            .AddJsonFile(AppsettingsJsonFilePath, JsonFileIsOptional)
            .AddEnvironmentVariables()
            .Build();

        _serviceProvider = TestCompositionRoot.BuildServiceProvider(configuration);
        ResolveServices();
        await Task.CompletedTask;
    }

    protected virtual void ResolveServices()
    {
        if (_serviceProvider == null) throw new InvalidOperationException("ServiceProvider not built.");
        PageNavigator = _serviceProvider.GetRequiredService<PageNavigator>();
        Playwright = _serviceProvider.GetRequiredService<IPlaywright>();
        Browser = _serviceProvider.GetRequiredService<IBrowser>();
        BrowserContext = _serviceProvider.GetRequiredService<IBrowserContext>();
        Page = _serviceProvider.GetRequiredService<IPage>();
        MediaWikiApiClient = _serviceProvider.GetRequiredService<IMediaWikiApiClient>();
    }

    protected T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null) throw new InvalidOperationException("ServiceProvider not built.");
        return _serviceProvider.GetRequiredService<T>();
    }

    protected virtual async Task AfterTestAsync()
    {
        await Page.CloseAsync();
        await Browser.CloseAsync();
        await BrowserContext.CloseAsync();
        Playwright.Dispose();
    }
}
