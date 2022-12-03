using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.BroadcasterCommandHandlers;

public class BonusCommandHandler : BroadcasterOnlyCommandHandler
{
    public BonusCommandHandler(GameSettings gameSettings,
    IRepository companyRepository, GameCommandArgs gameCommandArgs)
    : base("bonus", gameSettings, companyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        if (!GameCommandArgs.IsBroadcaster)
        {
            return;
        }

        var parsedArguments =
            _argumentParser.Parse(GameCommandArgs.Argument);

        if (parsedArguments.IntegerArguments.Count != 1 ||
            parsedArguments.IntegerArguments.First() < 1)
        {
            PublishMessage("Bonus command requires a single integer value over zero");
            return;
        }

        Logger.Trace($"Setting bonus of {parsedArguments.IntegerArguments.First()}");

        var bonusAmount = parsedArguments.IntegerArguments.First();

        RaiseBroadcasterCommandEvent(
            new BroadcasterCommandEventArgs(BroadcasterCommandType.Bonus, bonusAmount));
    }
}