using GenpactAutomation.Utils;
using Microsoft.Playwright;

namespace GenpactAutomation.Pages;

public class WikipediaPlaywrightPage : Page
{
    #region Selectors

    // Debugging features section
    private const string DebuggingFeaturesHeadingSelector = "#Debugging_features";
    private const string FollowingSiblingParagraphXPath = "xpath=following-sibling::p[1]";
    private const string FollowingSiblingListXPath = "xpath=following-sibling::ul[1]";

    // Microsoft development tools table
    private const string MicrosoftDevToolsTableSelector = "table:has([id^='Microsoft_development_tools'])";
    private const string TableDataCellLinksSelector = "td a";
    private const string TableDataCellsSelector = "td";

    // Theme/Appearance selectors
    private const string AppearanceToggleSelector = "#vector-appearance-dropdown-checkbox";
    private const string SkinThemePanelSelector = "#skin-client-prefs-skin-theme";
    private const string ThemeRadioSelectorTemplate = "input[name='skin-client-pref-skin-theme-group'][value='{0}']";
    private const string DarkThemeAppliedClass = "skin-theme-clientpref-night";

    #endregion

    #region Skip Texts for Technology Filtering

    private static readonly HashSet<string> SkipTexts = new(StringComparer.OrdinalIgnoreCase)
    {
        "v", "t", "e", "Category", "show", "hide"
    };

    private static readonly char[] TextSplitChars = { '·', '•', '\n', '\r' };

    #endregion

    public WikipediaPlaywrightPage(IPage page) : base(page)
    {
    }

    #region Task 1: Debugging Features Section

    /// <summary>
    /// Gets the "Debugging features" section body text.
    /// Navigates: #Debugging_features -> parent div.mw-heading -> following p and ul
    /// </summary>
    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        var headingDiv = PlaywrightPage.Locator(DebuggingFeaturesHeadingSelector).Locator("..");

        var paragraphText = await GetFollowingSiblingTextAsync(headingDiv, FollowingSiblingParagraphXPath);
        var listText = await GetFollowingSiblingTextAsync(headingDiv, FollowingSiblingListXPath);

        var rawText = $"{paragraphText} {listText}";
        return TextNormalizer.CleanWikipediaSectionText(rawText);
    }

    private async Task<string> GetFollowingSiblingTextAsync(ILocator parent, string xpath)
    {
        var sibling = parent.Locator(xpath);
        return await sibling.InnerTextAsync();
    }

    #endregion

    #region Task 2: Microsoft Development Tools Technology Entries

    /// <summary>
    /// Task 2: Gets all technology names from the "Microsoft development tools" table.
    /// Returns (Text, IsLink) for each technology name found in td cells.
    /// </summary>
    public async Task<IReadOnlyList<TechnologyEntry>> GetMicrosoftDevelopmentToolsTechnologyEntriesAsync()
    {
        var table = PlaywrightPage.Locator(MicrosoftDevToolsTableSelector);
        if (await table.CountAsync() == 0)
            return Array.Empty<TechnologyEntry>();

        var entries = new List<TechnologyEntry>();

        // Step 1: Extract all link entries from the table
        var linkEntries = await ExtractLinkEntriesAsync(table);
        entries.AddRange(linkEntries);

        // Step 2: Find non-link text entries in table cells
        var linkTexts = linkEntries.Select(e => e.Text).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var nonLinkEntries = await ExtractNonLinkEntriesAsync(table, linkTexts);
        entries.AddRange(nonLinkEntries);

        // Step 3: Deduplicate by text (case-insensitive)
        return DeduplicateEntries(entries);
    }

    private async Task<List<TechnologyEntry>> ExtractLinkEntriesAsync(ILocator table)
    {
        var entries = new List<TechnologyEntry>();
        var allLinks = table.Locator(TableDataCellLinksSelector);
        var linkCount = await allLinks.CountAsync();

        for (int i = 0; i < linkCount; i++)
        {
            var linkText = await GetLinkTextAsync(allLinks.Nth(i));

            if (IsValidTechnologyText(linkText))
            {
                entries.Add(new TechnologyEntry(linkText, IsLink: true));
            }
        }

        return entries;
    }

    private async Task<string> GetLinkTextAsync(ILocator link)
    {
        var text = await link.TextContentAsync();
        return (text ?? string.Empty).Trim();
    }

    private async Task<List<TechnologyEntry>> ExtractNonLinkEntriesAsync(ILocator table, HashSet<string> linkTexts)
    {
        var entries = new List<TechnologyEntry>();
        var tdCells = table.Locator(TableDataCellsSelector);
        var cellCount = await tdCells.CountAsync();

        for (int i = 0; i < cellCount; i++)
        {
            var cellText = await tdCells.Nth(i).InnerTextAsync();
            var nonLinkParts = ExtractNonLinkPartsFromCell(cellText, linkTexts);
            entries.AddRange(nonLinkParts);
        }

        return entries;
    }

    private IEnumerable<TechnologyEntry> ExtractNonLinkPartsFromCell(string cellText, HashSet<string> linkTexts)
    {
        var parts = cellText.Split(TextSplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var cleanPart = part.Trim();

            if (!IsValidTechnologyText(cleanPart))
                continue;

            if (!IsTextPresentInLinks(cleanPart, linkTexts))
            {
                yield return new TechnologyEntry(cleanPart, IsLink: false);
            }
        }
    }

    private static bool IsValidTechnologyText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (text.Length <= 1 || text == "·")
            return false;

        if (SkipTexts.Contains(text))
            return false;

        return true;
    }

    private static bool IsTextPresentInLinks(string text, HashSet<string> linkTexts)
    {
        return linkTexts.Any(lt =>
            lt.Equals(text, StringComparison.OrdinalIgnoreCase) ||
            lt.Contains(text, StringComparison.OrdinalIgnoreCase) ||
            text.Contains(lt, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<TechnologyEntry> DeduplicateEntries(List<TechnologyEntry> entries)
    {
        return entries
            .GroupBy(e => e.Text, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
    }

    #endregion

    #region Task 3: Color Theme

    /// <summary>
    /// Task 3: Selects the theme by name ("dark", "light", or "automatic").
    /// Opens the Appearance sidebar first, then clicks the theme radio.
    /// </summary>
    public async Task SetColorThemeAsync(string themeName)
    {
        var value = GetThemeValue(themeName);

        await OpenAppearanceSidebarAsync();
        await WaitForThemePanelVisibleAsync();
        await SelectThemeRadioAsync(value);
    }

    private static string GetThemeValue(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName) || !ThemeToValue.TryGetValue(themeName.Trim(), out var value))
        {
            throw new ArgumentException($"Theme must be one of: dark, light, automatic. Got: '{themeName}'", nameof(themeName));
        }

        return value;
    }

    private async Task OpenAppearanceSidebarAsync()
    {
        var appearanceToggle = PlaywrightPage.Locator(AppearanceToggleSelector);
        await appearanceToggle.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached, Timeout = 10_000 });
        await appearanceToggle.ClickAsync(new LocatorClickOptions { Force = true });
    }

    private async Task WaitForThemePanelVisibleAsync()
    {
        var skinTheme = PlaywrightPage.Locator(SkinThemePanelSelector);
        await skinTheme.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10_000 });
    }

    private async Task SelectThemeRadioAsync(string value)
    {
        var radioSelector = string.Format(ThemeRadioSelectorTemplate, value);
        var radio = PlaywrightPage.Locator(SkinThemePanelSelector).Locator(radioSelector);
        await radio.ClickAsync();
    }

    /// <summary>
    /// Task 3: Returns whether Wikipedia has applied dark theme (html has class skin-theme-clientpref-night).
    /// </summary>
    public async Task<bool> IsDarkThemeAppliedAsync()
    {
        return await PlaywrightPage.EvaluateAsync<bool>(
            $"() => document.documentElement.classList.contains('{DarkThemeAppliedClass}')");
    }

    #endregion

    #region Records & Mappings

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

    #endregion
}
