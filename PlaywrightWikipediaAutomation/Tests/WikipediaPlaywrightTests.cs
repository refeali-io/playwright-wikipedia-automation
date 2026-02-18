using Allure.NUnit.Attributes;
using PlaywrightWikipediaAutomation.Pages;
using PlaywrightWikipediaAutomation.Utils;
using NUnit.Framework;

namespace PlaywrightWikipediaAutomation.Tests;

[TestFixture]
[AllureSuite("Wikipedia Playwright")]
[AllureEpic("Genpact Automation")]
[AllureFeature("Wikipedia Tests")]
public class WikipediaPlaywrightTests : PlaywrightBaseTest
{
    /// <summary>
    /// Task 1: Extract "Debugging features" via UI (POM) and via API (MediaWiki Parse API),
    /// normalize both texts, count unique words, assert counts are equal.
    /// </summary>
    [Test]
    [AllureName("Task 1: Debugging features - UI and API unique word count match")]
    [AllureDescription("Extract the Debugging features section via UI (POM) and via API (MediaWiki Parse API), normalize both texts, count unique words, assert counts are equal.")]
    [AllureStory("Debugging Features Section")]
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
    [AllureName("Task 2: Microsoft development tools - all technology names are links")]
    [AllureDescription("Navigate to Microsoft development tools section (under Debugging features) and validate that all technology names are text links. Test fails and lists any non-link entries.")]
    [AllureStory("Microsoft Development Tools")]
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

    /// <summary>
    /// Task 3: Go to "Color (beta)" section, set theme to "Dark", assert the page applied dark theme.
    /// </summary>
    [Test]
    [AllureName("Task 3: Color (beta) - set to Dark and validate theme changed")]
    [AllureDescription("Open Color (beta) from the appearance sidebar, change theme to Dark, and validate that the page applied the dark theme (skin-theme-clientpref-night).")]
    [AllureStory("Color Theme")]
    public async Task Task3_ColorBeta_SetToDark_ThemeActuallyChanged()
    {
        var wikiPage = await PageNavigator.NavigateToAsync<WikipediaPlaywrightPage>();
        await wikiPage.SetColorThemeAsync("dark");

        var isDarkThemeApplied = await wikiPage.IsDarkThemeAppliedAsync();
        Assert.That(isDarkThemeApplied, Is.True, "Page should be in dark mode (html has skin-theme-clientpref-night).");
    }
}