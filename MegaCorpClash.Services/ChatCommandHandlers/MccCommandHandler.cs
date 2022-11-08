using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class MccCommandHandler : BaseCommandHandler
{
    public MccCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("mcc", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        PublishMessage(@"Instructions for MegaCorpClash are at https://megacorpclash.com/game-instructions/");
    }
}