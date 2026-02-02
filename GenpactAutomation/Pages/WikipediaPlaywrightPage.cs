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
    /// Task 2: Gets all technology names from the "Microsoft development tools" table.
    /// Returns (Text, IsLink) for each technology name found in td cells.
    /// </summary>
    public async Task<IReadOnlyList<TechnologyEntry>> GetMicrosoftDevelopmentToolsTechnologyEntriesAsync()
    {
        // Find the table containing "Microsoft_development_tools"
        var table = PlaywrightPage.Locator("table:has([id^='Microsoft_development_tools'])");
        if (await table.CountAsync() == 0)
            return Array.Empty<TechnologyEntry>();
        
        // Get all links inside td cells (technology names are in td, categories are in th)
        var allLinks = table.Locator("td a");
        var linkCount = await allLinks.CountAsync();
        var entries = new List<TechnologyEntry>();
        
        // Skip list: nav links, category, etc.
        var skipTexts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "v", "t", "e", "Category", "show", "hide"
        };
        
        for (int i = 0; i < linkCount; i++)
        {
            var link = allLinks.Nth(i);
            var text = (await link.TextContentAsync() ?? "").Trim();
            
            // Skip empty, separators, nav links, or non-tech entries
            if (string.IsNullOrWhiteSpace(text) || text == "·" || text.Length <= 1)
                continue;
            
            if (skipTexts.Contains(text))
                continue;
            
            entries.Add(new TechnologyEntry(text, IsLink: true));
        }
        
        // Check for any text in td cells that is NOT a link
        var tdCells = table.Locator("td");
        var cellCount = await tdCells.CountAsync();
        var linkTexts = entries.Select(e => e.Text).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        for (int i = 0; i < cellCount; i++)
        {
            var td = tdCells.Nth(i);
            var fullText = await td.InnerTextAsync();
            
            var parts = fullText.Split(new[] { '·', '•', '\n', '\r' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var part in parts)
            {
                var cleanPart = part.Trim();
                
                if (string.IsNullOrWhiteSpace(cleanPart) || cleanPart.Length <= 1)
                    continue;
                
                bool foundAsLink = linkTexts.Any(lt => 
                    lt.Equals(cleanPart, StringComparison.OrdinalIgnoreCase) ||
                    lt.Contains(cleanPart, StringComparison.OrdinalIgnoreCase) ||
                    cleanPart.Contains(lt, StringComparison.OrdinalIgnoreCase));
                
                if (!foundAsLink)
                    entries.Add(new TechnologyEntry(cleanPart, IsLink: false));
            }
        }
        
        // Remove duplicates
        return entries
            .GroupBy(e => e.Text, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
    }

    /// <summary>
    /// Represents a technology entry with its name and whether it's a link.
    /// </summary>
    public record TechnologyEntry(string Text, bool IsLink);

    /// <summary>
    /// Theme display names (user input) mapped to radio input value attributes.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string> ThemeToValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["dark"] = "night",
        ["light"] = "day",
        ["automatic"] = "os"
    };

    /// <summary>
    /// Task 3: Selects the theme by name ("dark", "light", or "automatic") via the radio under #skin-client-prefs-skin-theme.
    /// </summary>
    public async Task SetColorThemeAsync(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName) || !ThemeToValue.TryGetValue(themeName.Trim(), out var value))
            throw new ArgumentException($"Theme must be one of: dark, light, automatic. Got: '{themeName}'", nameof(themeName));

        // Open the theme menu so the radio is visible (it's in DOM but inside a collapsed panel)
        await PlaywrightPage.Locator("#skin-client-prefs-skin-theme").ClickAsync();
        await PlaywrightPage.GetByText("Color (beta)", new() { Exact = false }).ClickAsync();

        // Find the radio by value (night=dark, day=light, os=automatic) and click
        var radio = PlaywrightPage.Locator("#skin-client-prefs-skin-theme").Locator($"input[name='skin-client-pref-skin-theme-group'][value='{value}']");
        await radio.ClickAsync();
    }

    /// <summary>
    /// Task 3: Returns whether Wikipedia has applied dark theme (html has class skin-theme-clientpref-night).
    /// Use this to assert "we selected Dark and the page is in dark mode" regardless of OS preference.
    /// </summary>
    public async Task<bool> IsDarkThemeAppliedAsync()
    {
        return await PlaywrightPage.EvaluateAsync<bool>(@"() => document.documentElement.classList.contains('skin-theme-clientpref-night')");
    }
}