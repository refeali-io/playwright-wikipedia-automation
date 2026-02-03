using Microsoft.Playwright;

namespace GenpactAutomation.Elements;

public class Table : AbstractElement
{
    public Table(ILocator locator) : base(locator)
    {
    }

    public Table(IPage page, string selector, PageLocatorOptions? options = default) 
        : base(page, selector, options)
    {
    }

    /// <summary>
    /// Gets the tbody locator
    /// </summary>
    public ILocator TBody => Locator.Locator("tbody");

    /// <summary>
    /// Gets all rows in tbody
    /// </summary>
    public ILocator Rows => TBody.Locator("tr");

    /// <summary>
    /// Gets a specific row by index (0-based)
    /// </summary>
    public ILocator GetRow(int index) => Rows.Nth(index);

    /// <summary>
    /// Gets all cells in a row
    /// </summary>
    public ILocator GetCells(ILocator row) => row.Locator("td, th");

    /// <summary>
    /// Gets the inner text of tbody
    /// </summary>
    public async Task<string> GetTBodyTextAsync() => await TBody.InnerTextAsync();

    /// <summary>
    /// Gets the inner text of the entire table
    /// </summary>
    public async Task<string> GetTableTextAsync() => await Locator.InnerTextAsync();
}
