namespace MegaCorpClash.Core;

public class ArgumentParser
{
    private readonly char[] _separators;

    public ArgumentParser() : this(new[] { ' ' })
    {
    }

    public ArgumentParser(char[] separators)
    {
        _separators = separators;
    }

    public ParsedArguments Parse(string arguments)
    {
        // Initial split of arguments
        var splitArguments =
            arguments.Split(_separators,
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        // Find arguments that are integers or decimals
        var integerArgumentsAsStrings =
            splitArguments
                .Where(a => int.TryParse(a, out _))
                .ToList();

        var decimalArgumentsAsStrings =
            splitArguments
                .Except(integerArgumentsAsStrings)
                .Where(a => decimal.TryParse(a, out _))
                .ToList();

        // Variables for arguments by datatype
        var integerArguments =
            integerArgumentsAsStrings
                .Select(int.Parse)
                .ToList();

        var decimalArguments =
            decimalArgumentsAsStrings
                .Select(decimal.Parse)
                .ToList();

        var stringArguments =
            splitArguments
                .Except(integerArgumentsAsStrings)
                .Except(decimalArgumentsAsStrings)
                .ToList();

        return new
            ParsedArguments(
                splitArguments,
                integerArguments,
                decimalArguments,
                stringArguments);
    }
}