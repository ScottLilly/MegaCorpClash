using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.BroadcasterCommandHandlers;

public class TaxCommandHandler : BroadcasterOnlyCommandHandler
{
    public TaxCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("tax", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        if (!gameCommandArgs.IsBroadcaster)
        {
            return;
        }

        var parsedArguments =
            _argumentParser.Parse(gameCommandArgs.Argument);

        if (parsedArguments.IntegerArguments.Count != 1 ||
            parsedArguments.IntegerArguments.First() < 1 ||
            parsedArguments.IntegerArguments.First() > 99)
        {
            PublishMessage("Tax command requires a single integer tax rate between 1 and 99");
            return;
        }

        foreach (var company in Companies
            .Where(c => !c.Value.IsBroadcaster)
            .Select(c => c.Value))
        {
            company.Points =
                Convert.ToInt32(
                    company.Points *
                    ((100M - parsedArguments.IntegerArguments.First()) / 100M));
        }

        PublishMessage($"A tax of {parsedArguments.IntegerArguments.First()} has been applied");

        NotifyPlayerDataUpdated();
    }
}