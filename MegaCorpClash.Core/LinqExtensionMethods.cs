namespace MegaCorpClash.Core;

public static class LinqExtensionMethods
{
    public static bool None<T>(this IEnumerable<T> elements, Func<T, bool>? func = null)
    {
        return func == null
            ? !elements.Any()
            : !elements.Any(func.Invoke);
    }

    public static void ApplyToEach<T>(this IEnumerable<T> elements, Action<T> func)
    {
        foreach (T element in elements)
        {
            func(element);
        }
    }

    public static T? RandomElement<T>(this List<T> options)
    {
        return options.None() 
            ? default 
            : options[RngCreator.GetNumberBetween(0, options.Count - 1)];
    }
}