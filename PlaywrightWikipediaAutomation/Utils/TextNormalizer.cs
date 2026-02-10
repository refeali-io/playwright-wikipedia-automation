using System.Text.RegularExpressions;

namespace PlaywrightWikipediaAutomation.Utils;

public static class TextNormalizer
{
    private static readonly Regex WordSplit = new(@"\W+", RegexOptions.Compiled);
    
    // Matches citation markers like [1], [2], &#91;1&#93; etc.
    private static readonly Regex CitationMarker = new(@"(\[|\&#91;)\s*\d+\s*(\]|\&#93;)", RegexOptions.Compiled);
    
    // Matches [ edit ] or [edit]
    private static readonly Regex EditLink = new(@"\[\s*edit\s*\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Matches HTML entities like &#91; &#93; &nbsp; etc.
    private static readonly Regex HtmlEntities = new(@"&[#\w]+;", RegexOptions.Compiled);

    /// <summary>
    /// Cleans Wikipedia section text by removing:
    /// - Everything after citation markers [1], [2], etc.
    /// - [ edit ] links
    /// - Section headings (if provided)
    /// - HTML entities
    /// </summary>
    public static string CleanWikipediaSectionText(string? text, string? headingToRemove = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var cleaned = text;
        
        // Remove heading if provided (e.g., "Debugging features")
        if (!string.IsNullOrEmpty(headingToRemove))
            cleaned = Regex.Replace(cleaned, Regex.Escape(headingToRemove), " ", RegexOptions.IgnoreCase);
        
        // Remove [ edit ] links
        cleaned = EditLink.Replace(cleaned, " ");
        
        // Truncate at first citation marker [1] or &#91;1&#93;
        var citationMatch = CitationMarker.Match(cleaned);
        if (citationMatch.Success)
            cleaned = cleaned.Substring(0, citationMatch.Index);
        
        // Decode/remove common HTML entities
        cleaned = HtmlEntities.Replace(cleaned, " ");
        
        // Normalize whitespace
        cleaned = Regex.Replace(cleaned, @"\s+", " ");
        
        return cleaned.Trim();
    }

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
