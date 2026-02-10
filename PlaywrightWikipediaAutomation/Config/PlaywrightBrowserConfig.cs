using Microsoft.Playwright;
using static Microsoft.Playwright.BrowserType;

namespace PlaywrightWikipediaAutomation.Config;

public class PlaywrightBrowserConfig : BrowserTypeLaunchOptions
{
    public string BrowserType { get; set; } = Chromium;

}