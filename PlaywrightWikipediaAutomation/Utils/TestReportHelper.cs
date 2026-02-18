using static PlaywrightWikipediaAutomation.Pages.WikipediaPlaywrightPage;

namespace PlaywrightWikipediaAutomation.Utils;

/// <summary>
/// Helper for formatting test output reports.
/// </summary>
public static class TestReportHelper
{
    /// <summary>Total width of the report box (title + padding).</summary>
    private const int BoxWidth = 62;
    /// <summary>Max width for technology name in a row.</summary>
    private const int TextColumnWidth = 48;

    /// <summary>
    /// Prints a formatted report of technology entries with their link status.
    /// </summary>
    public static void PrintTechnologyEntriesReport(
        IReadOnlyList<TechnologyEntry> entries, 
        string title = "TECHNOLOGY ENTRIES")
    {
        var linkEntries = entries.Where(e => e.IsLink).ToList();
        var nonLinkEntries = entries.Where(e => !e.IsLink).ToList();
        
        var line = new string('─', BoxWidth);
        Console.WriteLine();
        Console.WriteLine($"┌{line}┐");
        Console.WriteLine($"│  {PadRight(Truncate(title, BoxWidth - 4), BoxWidth - 4)}│");
        Console.WriteLine($"│  Total: {PadRight(entries.Count.ToString(), BoxWidth - 12)}│");
        Console.WriteLine($"├{line}┤");
        Console.WriteLine($"│  Links:     {PadRight(linkEntries.Count.ToString(), BoxWidth - 16)}│");
        Console.WriteLine($"│  Not Links: {PadRight(nonLinkEntries.Count.ToString(), BoxWidth - 16)}│");
        Console.WriteLine($"├{line}┤");
        
        foreach (var entry in entries)
        {
            var status = entry.IsLink ? "[LINK]    " : "[NOT LINK]";
            var displayText = Truncate(entry.Text, TextColumnWidth);
            Console.WriteLine($"│  {status} {PadRight(displayText, TextColumnWidth)}│");
        }
        
        Console.WriteLine($"└{line}┘");
        Console.WriteLine();
    }
    
    private static string PadRight(string value, int totalWidth)
    {
        if (string.IsNullOrEmpty(value)) return new string(' ', totalWidth);
        if (value.Length >= totalWidth) return value.Substring(0, totalWidth);
        return value + new string(' ', totalWidth - value.Length);
    }
    
    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value ?? "";
        return value.Substring(0, maxLength - 3) + "...";
    }
    
    /// <summary>
    /// Prints only the non-link entries (failures).
    /// </summary>
    public static void PrintNonLinkEntries(IReadOnlyList<TechnologyEntry> entries)
    {
        var nonLinkEntries = entries.Where(e => !e.IsLink).ToList();
        
        if (!nonLinkEntries.Any())
        {
            Console.WriteLine("All technology names are links.");
            return;
        }
        
        Console.WriteLine();
        Console.WriteLine("Technology names that are NOT links:");
        foreach (var entry in nonLinkEntries)
        {
            Console.WriteLine($"  - {entry.Text}");
        }
        Console.WriteLine();
    }
}
