namespace PlaywrightWikipediaAutomation.Api;

public interface IMediaWikiApiClient
{
    /// <summary>
    /// Gets the "Debugging features" section body text as plain text (HTML stripped).
    /// Uses MediaWiki Parse API for page Playwright_(software).
    /// </summary>
    Task<string> GetDebuggingFeaturesSectionTextAsync(CancellationToken cancellationToken = default);
}
