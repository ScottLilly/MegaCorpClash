namespace MegaCorpClash.Core;

public class ParsedArguments
{
    public List<string> Arguments { get; }

    public List<int> IntegerArguments { get; }
    public List<decimal> DecimalArguments { get; }
    public List<string> StringArguments { get; }

    // By only trying to parse the StringArguments to the enum,
    // this will ignore integer parameters which could map to an enum value.
    public IEnumerable<T> EnumArgumentsOfType<T>() where T : struct =>
        StringArguments.Where(a => Enum.TryParse(a, true, out T _))
            .Select(a => (T)Enum.Parse(typeof(T), a, true));

    public ParsedArguments(List<string> arguments,
        List<int> integerArguments, List<decimal> decimalArguments,
        List<string> stringArguments)
    {
        Arguments = arguments;
        IntegerArguments = integerArguments;
        DecimalArguments = decimalArguments;
        StringArguments = stringArguments;
    }
}