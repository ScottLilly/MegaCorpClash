using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.BroadcasterCommandHandlers;

public class MultiplierCommandHandler : BroadcasterOnlyCommandHandler
{
    public MultiplierCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("multiplier", gameSettings, companyRepository, gameCommandArgs)
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
            PublishMessage("Multiplier command requires a single integer value over zero");
            return;
        }

        Logger.Trace($"Setting multipler of {parsedArguments.IntegerArguments.First()}");

        var multiplier = parsedArguments.IntegerArguments.First();

        RaiseBroadcasterCommandEvent(
            new BroadcasterCommandEventArgs(BroadcasterCommandType.Multiplier, multiplier));
    }
}