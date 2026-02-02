namespace GenpactAutomation.Config;

public class MediaWikiApiConfig
{
    /// <summary>
    /// Route API base URL (e.g. https://en.wikipedia.org/w/api.php). Set from WikipediaConfig in post-configure.
    /// </summary>
    public string MediaWikiPhpRoute { get; set; } = string.Empty;
}
