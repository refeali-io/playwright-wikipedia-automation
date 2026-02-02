namespace GenpactAutomation.Config;

/// <summary>
/// Single shared Wikipedia base URL. All other configs derive from this.
/// </summary>
public class WikipediaConfig
{
    public string BaseUrl { get; set; } = "https://en.wikipedia.org/";
}
