namespace PlaywrightWikipediaAutomation.Config;

public class MediaWikiApiConfig
{
    /// <summary>
    /// Base URL for Wikipedia - populated from WikipediaConfig
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Route to the API endpoint (e.g. w/api.php)
    /// </summary>
    public string MediaWikiPhpRoute { get; set; } = "w/api.php";
    
    /// <summary>
    /// Gets the full API URL by combining BaseUrl and route
    /// </summary>
    public string GetFullApiUrl() => $"{BaseUrl.TrimEnd('/')}/{MediaWikiPhpRoute.TrimStart('/')}";
}
