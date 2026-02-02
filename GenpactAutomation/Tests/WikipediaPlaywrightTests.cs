using GenpactAutomation.Pages;
using GenpactAutomation.Utils;
using NUnit.Framework;

namespace GenpactAutomation.Tests;

[TestFixture]
public class WikipediaPlaywrightTests : PlaywrightBaseTest
{
    /// <summary>
    /// Task 1: Extract "Debugging features" via UI (POM) and via API (MediaWiki Parse API),
    /// normalize both texts, count unique words, assert counts are equal.
    /// </summary>
    [Test]
    public async Task Task1_DebuggingFeaturesSection_UniqueWordCount_UI_Equals_API()
    {
        var wikiPage = await PageNavigator.NavigateToAsync<WikipediaPlaywrightPage>();
        var uiText = await wikiPage.GetDebuggingFeaturesSectionTextAsync();
        var apiText = await MediaWikiApiClient.GetDebuggingFeaturesSectionTextAsync();

        var uiUniqueCount = TextNormalizer.CountUniqueWords(uiText);
        var apiUniqueCount = TextNormalizer.CountUniqueWords(apiText);

        Assert.That(apiUniqueCount, Is.EqualTo(uiUniqueCount),
            $"Unique word count mismatch: UI={uiUniqueCount}, API={apiUniqueCount}. " +
            "Both 'Debugging features' section texts should yield the same unique word count after normalization.");
    }

    /// <summary>
    /// Task 2: Go to Microsoft development tools section and validate all technology names are links.
    /// </summary>
    [Test]
    public async Task Task2_MicrosoftDevelopmentTools_AllTechnologyNames_AreLinks()
    {
        var wikiPage = await PageNavigator.NavigateToAsync<WikipediaPlaywrightPage>();
        var entries = await wikiPage.GetMicrosoftDevelopmentToolsTechnologyEntriesAsync();
        
        // Print formatted report
        TestReportHelper.PrintTechnologyEntriesReport(entries, "MICROSOFT DEVELOPMENT TOOLS");
        
        // Verify we found entries
        Assert.That(entries, Is.Not.Empty, "Microsoft development tools table should contain technology entries.");
        
        // Assert all entries are links
        var nonLinkEntries = entries.Where(e => !e.IsLink).ToList();
        Assert.That(nonLinkEntries, Is.Empty,
            $"All technology names must be text links. Found {nonLinkEntries.Count} non-link entries:\n" +
            string.Join("\n", nonLinkEntries.Select(e => $"  - {e.Text}")));
    }

    // /// <summary>
    // /// Task 3: Go to "Color (beta)" section (from the right), change to "Dark", validate that the color actually changed.
    // /// </summary>
    // [Test]
    // public async Task Task3_ColorBeta_SetToDark_ThemeActuallyChanged()
    // {
    //     var wikiPage = await PageNavigator.NavigateToAsync<WikipediaPlaywrightPage>();
    //     await wikiPage.SetColorThemeToDarkAsync();
    //     var isDark = await wikiPage.IsDarkThemeActiveAsync();
    //     Assert.That(isDark, Is.True, "Color theme should be Dark (skin-theme-clientpref-night) after selecting Dark.");
    // }
}