using System.Text.RegularExpressions;

namespace GenpactAutomation.Utils;

public static class TextNormalizer
{
    private static readonly Regex WordSplit = new(@"\W+", RegexOptions.Compiled);

    /// <summary>
    /// Normalizes text: lowercase, trim, split on non-word characters.
    /// Returns the words (empty entries removed).
    /// </summary>
    public static IReadOnlyList<string> NormalizeToWords(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<string>();

        var normalized = text.Trim().ToLowerInvariant();
        var words = WordSplit.Split(normalized);
        return words.Where(w => w.Length > 0).ToArray();
    }

    /// <summary>
    /// Counts unique words after normalizing (lowercase, non-word split).
    /// </summary>
    public static int CountUniqueWords(string? text)
    {
        var words = NormalizeToWords(text);
        return words.Distinct().Count();
    }
}
