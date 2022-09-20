namespace MegaCorpClash.Core;

public static class ExtensionMethods
{
    public static bool IsSafeText(this string text)
    {
        return text.All(c => 
            "abcdefghijklmnopqrstuvwxyz".IndexOf(c.ToString().ToLowerInvariant()) > -1 ||
            char.IsDigit(c) || 
            c is ',' or ' ' or '-' or '&' or '!' or '.' or '\'');
    }

    public static bool IsNotSafeText(this string text)
    {
        return !text.IsSafeText();
    }
}