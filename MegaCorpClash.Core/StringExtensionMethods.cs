namespace MegaCorpClash.Core;

public static class StringExtensionMethods
{
    private static readonly List<string> s_lowerCaseWords =
        new()
        {
            "a",
            "an",
            "and",
            "as",
            "as long as",
            "at",
            "but",
            "by",
            "even if",
            "for",
            "from",
            "if",
            "if only",
            "in",
            "is",
            "into",
            "like",
            "near",
            "now that",
            "nor",
            "of",
            "off",
            "on",
            "on top of",
            "once",
            "onto",
            "or",
            "out of",
            "over",
            "past",
            "so",
            "so that",
            "than",
            "that",
            "the",
            "till",
            "to",
            "up",
            "upon",
            "with",
            "when",
            "yet"
        };

    /// <summary>
    /// Check if strings are equal, using InvariantCultureIgnoreCase
    /// </summary>
    /// <param name="text"></param>
    /// <param name="matchingText"></param>
    /// <returns>True, if string match. False, if they don't.</returns>
    public static bool Matches(this string text, string matchingText)
    {
        return text.Equals(matchingText, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Check if strings are not equal, using InvariantCultureIgnoreCase
    /// </summary>
    /// <param name="text"></param>
    /// <param name="comparisonText"></param>
    /// <returns>True, if strings do not match. False, if they do.</returns>
    public static bool DoesNotMatch(this string text, string comparisonText)
    {
        return !text.Matches(comparisonText);
    }

    /// <summary>
    /// Get string with the first character of each word in correct book title casing
    /// e.g. convert "a tale of two cities" to "A Tale of Two Cities"
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Returns a copy of this string,
    /// with the first character of each word in correct book title casing</returns>
    public static string ToTitleCase(this string text)
    {
        string cleanedText =
            text
                .Replace("-", " ")
                .Replace("_", " ");

        var rawWords =
            cleanedText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().UpperCaseFirstChar())
                .ToList();

        var properCasedWords = new List<string>();

        if (rawWords.Any())
        {
            // First word is always upper-case
            properCasedWords.Add(rawWords.First().UpperCaseFirstChar());

            // Find correct casing for subsequent words
            foreach (string word in rawWords.Skip(1))
            {
                var matchingLowerCaseWord =
                    s_lowerCaseWords.FirstOrDefault(lcw =>
                        word.Equals(lcw, StringComparison.InvariantCultureIgnoreCase));

                properCasedWords.Add(matchingLowerCaseWord ?? word.UpperCaseFirstChar());
            }
        }

        return string.Join(' ', properCasedWords);
    }

    #region Private methods

    private static string UpperCaseFirstChar(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return new string(a);
    }

    #endregion
}