using System.Threading.Tasks;
using GenpactAutomation.Extensions;
using GenpactAutomation.Navigation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace GenpactAutomation;

public abstract class SdkPlaywrightBaseTest
{
    protected TestServer TestServer = null!;
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext BrowserContext = null!;
    protected IPage Page = null!;
    protected PageNavigator PageNavigator = null!;

    protected virtual string AppsettingsJsonFilePath => "appsettings.development.json";
    private const bool JSON_FILE_IS_OPTIONAL = false;

    protected virtual async Task BeforeTestAsync()
    {
        TestServer = InitializeTestServer();
        InitializeServices();
        await Task.CompletedTask;
    }

    protected virtual async Task AfterTestAsync()
    {
        await Page.CloseAsync();
        await Browser.CloseAsync();
        await BrowserContext.CloseAsync();
        Playwright.Dispose();
    }

    protected virtual TestServer InitializeTestServer()
    {
        var configurationBuilder = new ConfigurationBuilder().AddDefaultAppsettings();
        var configuration = ConfigureJsonFiles(configurationBuilder).AddEnvironmentVariables().Build();
        var webHostBuilder = new WebHostBuilder()
            .UseConfiguration(configuration)
            .UseStartup<Startup>()
            .ConfigureTestServices(ConfigureServices);

        return new TestServer(webHostBuilder);
    }

    protected virtual void ConfigureServices(IServiceCollection service)
    {
    }

    protected virtual IConfigurationBuilder ConfigureJsonFiles(IConfigurationBuilder builder)
    {
        builder.AddJsonFile(AppsettingsJsonFilePath, JSON_FILE_IS_OPTIONAL);
        return builder;
    }

    protected virtual void InitializeServices()
    {
        PageNavigator = TestServer.Services.GetRequiredService<PageNavigator>();
        Playwright = TestServer.Services.GetRequiredService<IPlaywright>();
        Browser = TestServer.Services.GetRequiredService<IBrowser>();
        BrowserContext = TestServer.Services.GetRequiredService<IBrowserContext>();
        Page = TestServer.Services.GetRequiredService<IPage>();
    }
}
