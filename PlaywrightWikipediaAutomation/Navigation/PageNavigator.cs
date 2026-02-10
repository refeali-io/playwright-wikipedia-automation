using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaywrightWikipediaAutomation.Config;
using PlaywrightWikipediaAutomation.Pages;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace PlaywrightWikipediaAutomation.Navigation;

public class PageNavigator
{
    private readonly IPage _page;
    private readonly IEnumerable<Page> _pages;
    private readonly PageNavigatorConfig _pageNavigatorConfig;

    public PageNavigator(IPage page, IEnumerable<Page> pages, IOptions<PageNavigatorConfig> pageNavigatorConfig)
    {
        _page = page;
        _pages = pages;
        _pageNavigatorConfig = pageNavigatorConfig.Value;
    }

    public async Task RefreshAsync()
    {
        await _page.ReloadAsync();
    }

    public async Task<T> NavigateToAsync<T>() where T : Page
    {
        await NavigateToPageAsync(ExtractPageRoute<T>());
        return ExtractPage<T>();
    }

    private string ExtractPageRoute<T>()
    {
        return _pageNavigatorConfig.GetType().GetProperties()
                   .First(property => property.Name.Contains(typeof(T).Name, StringComparison.OrdinalIgnoreCase))
                   .GetValue(_pageNavigatorConfig) as string
               ?? throw new InvalidOperationException($"No route found for page type {typeof(T).Name}");
    }

    private async Task NavigateToPageAsync(string route)
    {
        var baseUrl = _pageNavigatorConfig.BaseUrl.TrimEnd('/');
        var fullUrl = $"{baseUrl}/{route.TrimStart('/')}";
        await _page.GotoAsync(fullUrl);
    }

    private T ExtractPage<T>() where T : Page
    {
        return _pages.First(page => page.GetType() == typeof(T)) as T
               ?? throw new InvalidOperationException($"Page type {typeof(T).Name} not registered");
    }
}