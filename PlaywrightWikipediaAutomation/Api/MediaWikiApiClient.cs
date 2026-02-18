using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PlaywrightWikipediaAutomation.Utils;

namespace PlaywrightWikipediaAutomation.Api;

public class MediaWikiApiClient : IMediaWikiApiClient
{
    private const string PageTitle = "Playwright_(software)";
    private const string SectionHeading = "Debugging features";

    private readonly HttpClient _httpClient;

    public MediaWikiApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetDebuggingFeaturesSectionTextAsync(CancellationToken cancellationToken = default)
    {
        var sectionIndex = await GetSectionIndexAsync(cancellationToken).ConfigureAwait(false);
        var html = await GetSectionHtmlAsync(sectionIndex, cancellationToken).ConfigureAwait(false);
        var rawText = StripHtml(html);
        
        // Clean: remove heading, [edit], and truncate at citations [1]
        return TextNormalizer.CleanWikipediaSectionText(rawText, SectionHeading);
    }

    private async Task<int> GetSectionIndexAsync(CancellationToken cancellationToken)
    {
        var route = $"?action=parse&page={Uri.EscapeDataString(PageTitle)}&prop=sections&format=json&origin=*";
        var response = await _httpClient.GetFromJsonAsync<MediaWikiParseSectionsResponse>(route, cancellationToken).ConfigureAwait(false);
        if (response?.Parse?.Sections == null)
            throw new InvalidOperationException("MediaWiki API did not return sections.");

        var section = response.Parse.Sections.FirstOrDefault(s =>
            string.Equals(s.Line, SectionHeading, StringComparison.OrdinalIgnoreCase));
        if (section == null)
            throw new InvalidOperationException($"Section '{SectionHeading}' not found.");

        return section.Index;
    }

    private async Task<string> GetSectionHtmlAsync(int sectionIndex, CancellationToken cancellationToken)
    {
        var route = $"?action=parse&page={Uri.EscapeDataString(PageTitle)}&section={sectionIndex}&prop=text&format=json&origin=*";
        var response = await _httpClient.GetFromJsonAsync<MediaWikiParseTextResponse>(route, cancellationToken).ConfigureAwait(false);
        if (response?.Parse?.Text == null)
            throw new InvalidOperationException("MediaWiki API did not return section text.");

        return response.Parse.Text.Value;
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var text = Regex.Replace(html, "<[^>]+>", " ");
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    private class MediaWikiParseSectionsResponse
    {
        [JsonPropertyName("parse")]
        public MediaWikiParse Parse { get; set; } = null!;
    }

    private class MediaWikiParseTextResponse
    {
        [JsonPropertyName("parse")]
        public MediaWikiParse Parse { get; set; } = null!;
    }

    private class MediaWikiParse
    {
        [JsonPropertyName("sections")]
        public List<MediaWikiSection>? Sections { get; set; }
        [JsonPropertyName("text")]
        public MediaWikiParseText? Text { get; set; }
    }

    private class MediaWikiSection
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("line")]
        public string Line { get; set; } = string.Empty;
    }

    private class MediaWikiParseText
    {
        [JsonPropertyName("*")]
        public string Value { get; set; } = string.Empty;
    }
}
