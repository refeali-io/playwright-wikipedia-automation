using Microsoft.Playwright;
using static Microsoft.Playwright.BrowserType;

namespace GenpactAutomation.Config;

public class PlaywrightBrowserConfig : BrowserTypeLaunchOptions
{
    public string BrowserType { get; set; } = Chromium;

}