using System.Text.Json.Serialization;
using GenpactAutomation.Utils;
using Microsoft.Playwright;

namespace GenpactAutomation.Pages;

public class WikipediaPlaywrightPage : Page
{
    public WikipediaPlaywrightPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Gets the "Debugging features" section body text.
    /// Navigates: #Debugging_features -> parent div.mw-heading -> following <p> and <ul>
    /// </summary>
    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        // Find the h3 heading, go up to parent div.mw-heading
        var headingDiv = PlaywrightPage.Locator("#Debugging_features").Locator("..");
        
        // Get the <p> that follows the heading div
        var paragraph = headingDiv.Locator("xpath=following-sibling::p[1]");
        var paragraphText = await paragraph.InnerTextAsync();
        
        // Get the <ul> that follows the heading div
        var list = headingDiv.Locator("xpath=following-sibling::ul[1]");
        var listText = await list.InnerTextAsync();
        
        var rawText = $"{paragraphText} {listText}";
        
        // Clean: remove any stray [edit], citations, etc.
        return TextNormalizer.CleanWikipediaSectionText(rawText);
    }

    /// <summary>
    /// Task 2: Gets all "technology names" in the "Microsoft development tools" subsection (under Debugging features).
    /// Returns (text, isLink) for each; test should assert all are links.
    /// </summary>
    public async Task<IReadOnlyList<(string Text, bool IsLink)>> GetMicrosoftDevelopmentToolsTechnologyEntriesAsync()
    {
        var raw = await PlaywrightPage.EvaluateAsync<List<TechnologyEntryDto>>(@"() => {
            const sectionEl = document.getElementById('Microsoft_development_tools');
            if (!sectionEl) return [];
            const container = sectionEl.closest('h3')?.nextElementSibling || sectionEl.closest('h2')?.nextElementSibling;
            if (!container) return [];
            const listItems = container.querySelectorAll('li');
            const result = [];
            for (const li of listItems) {
                const link = li.querySelector('a');
                const text = (link || li).innerText.trim();
                if (!text) continue;
                result.push({ text: text, isLink: !!link });
            }
            return result;
        }");
        if (raw == null) return Array.Empty<(string, bool)>();
        return raw.ConvertAll(e => (e.Text, e.IsLink));
    }

    private class TechnologyEntryDto
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        [JsonPropertyName("isLink")]
        public bool IsLink { get; set; }
    }

    /// <summary>
    /// Task 3: Opens the Color (beta) / Appearance section in the sidebar and selects "Dark".
    /// </summary>
    public async Task SetColorThemeToDarkAsync()
    {
        await PlaywrightPage.Locator("#vector-appearance-dropdown, a[title='Appearance'], [data-event-name='ui.appearance']").First.ClickAsync();
        await PlaywrightPage.GetByText("Color (beta)", new() { Exact = false }).ClickAsync();
        await PlaywrightPage.GetByRole(AriaRole.Option, new() { Name = "Dark" }).Or(PlaywrightPage.GetByText("Dark").First).ClickAsync();
    }

    /// <summary>
    /// Task 3: Returns whether dark theme is currently active (html has skin-theme-clientpref-night).
    /// </summary>
    public async Task<bool> IsDarkThemeActiveAsync()
    {
        return await PlaywrightPage.EvaluateAsync<bool>(@"() => document.documentElement.classList.contains('skin-theme-clientpref-night')");
    }
}