using Microsoft.Playwright;

namespace PlaywrightWikipediaAutomation.Pages;

public abstract class Page
{
    protected readonly IPage PlaywrightPage;

    protected Page(IPage page)
    {
        PlaywrightPage = page;
    }
}