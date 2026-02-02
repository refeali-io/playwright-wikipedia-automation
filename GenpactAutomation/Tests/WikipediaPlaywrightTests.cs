using System.Threading.Tasks;
using GenpactAutomation.Pages;
using NUnit.Framework;

namespace GenpactAutomation.Tests;

[TestFixture]
public class WikipediaPlaywrightTests : PlaywrightBaseTest
{
    [Test]
    public async Task NavigateToWikipediaPlaywrightPage_Succeeds()
    {
        var page = await PageNavigator.NavigateToAsync<WikipediaPlaywrightPage>();
        Assert.That(page, Is.Not.Null);
    }
}