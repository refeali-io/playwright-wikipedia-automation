using Microsoft.Playwright;

namespace GenpactAutomation.Pages;

public abstract class Page
{
    protected readonly IPage PlaywrightPage;

    protected Page(IPage page)
    {
        PlaywrightPage = page;
    }
}