namespace PlaywrightWikipediaAutomation.Config;

public class PageNavigatorConfig
{
    /// <summary>
    /// Base URL for Wikipedia - populated from WikipediaConfig
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Route to the Playwright page (e.g. wiki/Playwright_(software))
    /// </summary>
    public string WikipediaPlaywrightPageRoute { get; set; } = string.Empty;
}
