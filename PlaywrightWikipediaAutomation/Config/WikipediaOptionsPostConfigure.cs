using Microsoft.Extensions.Options;

namespace PlaywrightWikipediaAutomation.Config;

/// <summary>
/// Post-configures PageNavigatorConfig and MediaWikiApiConfig with the shared Wikipedia base URL.
/// </summary>
public class WikipediaOptionsPostConfigure : 
    IPostConfigureOptions<PageNavigatorConfig>, 
    IPostConfigureOptions<MediaWikiApiConfig>
{
    private readonly WikipediaConfig _wikipedia;

    public WikipediaOptionsPostConfigure(IOptions<WikipediaConfig> wikipedia)
    {
        _wikipedia = wikipedia.Value;
    }

    public void PostConfigure(string? name, PageNavigatorConfig options)
    {
        if (string.IsNullOrEmpty(options.BaseUrl))
            options.BaseUrl = _wikipedia.BaseUrl;
    }

    public void PostConfigure(string? name, MediaWikiApiConfig options)
    {
        if (string.IsNullOrEmpty(options.BaseUrl))
            options.BaseUrl = _wikipedia.BaseUrl;
    }
}
