using System.Threading.Tasks;
using Microsoft.Playwright;

namespace GenpactAutomation.Pages;

public class WikipediaPlaywrightPage : Page
{
    public WikipediaPlaywrightPage(IPage page) : base(page)
    {
    }

    /// <summary>
    /// Gets the "Debugging features" section body text (from the h2 until the next h2).
    /// Wikipedia uses id="Debugging_features" on the section heading span.
    /// </summary>
    public async Task<string> GetDebuggingFeaturesSectionTextAsync()
    {
        return await PlaywrightPage.EvaluateAsync<string>(@"() => {
            const el = document.getElementById('Debugging_features');
            if (!el) return '';
            const h2 = el.closest('h2');
            if (!h2) return '';
            let s = '';
            let next = h2.nextElementSibling;
            while (next && next.tagName !== 'H2') {
                s += next.innerText + ' ';
                next = next.nextElementSibling;
            }
            return s.trim();
        }");
    }
}