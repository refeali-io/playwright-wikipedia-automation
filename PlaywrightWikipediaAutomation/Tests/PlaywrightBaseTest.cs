using Allure.NUnit;
using NUnit.Framework;

namespace PlaywrightWikipediaAutomation.Tests;

[TestFixture]
[AllureNUnit]
public abstract class PlaywrightBaseTest : SdkPlaywrightBaseTest
{
    [SetUp]
    public async Task SetUp()
    {
        await BeforeTestAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await AfterTestAsync();
    }
}