using Microsoft.Playwright;

namespace GenpactAutomation.Elements;

public abstract class AbstractElement
{
    protected ILocator Locator { get; }

    protected AbstractElement(ILocator locator)
    {
        Locator = locator;
    }

    protected AbstractElement(IPage page, string selector, PageLocatorOptions? options = default) :
        this(page.Locator(selector, options))
    {
    }
}
