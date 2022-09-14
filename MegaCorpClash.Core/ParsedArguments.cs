using System.Collections.ObjectModel;

namespace MegaCorpClash.Core;

public sealed class ParsedArguments
{
    public ReadOnlyCollection<string> Arguments { get; }

    public ReadOnlyCollection<int> IntegerArguments { get; }
    public ReadOnlyCollection<decimal> DecimalArguments { get; }
    public ReadOnlyCollection<string> StringArguments { get; }

    // By only trying to parse the StringArguments to the enum,
    // this will ignore integer parameters which could map to an enum value.
    public IEnumerable<T> EnumArgumentsOfType<T>() where T : struct =>
        StringArguments.Where(a => Enum.TryParse(a, true, out T _))
            .Select(a => (T)Enum.Parse(typeof(T), a, true));

    public ParsedArguments(List<string> arguments,
        List<int> integerArguments, List<decimal> decimalArguments,
        List<string> stringArguments)
    {
        Arguments = arguments.AsReadOnly();
        IntegerArguments = integerArguments.AsReadOnly();
        DecimalArguments = decimalArguments.AsReadOnly();
        StringArguments = stringArguments.AsReadOnly();
    }
}