using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class MccCommandHandler : BaseCommandHandler
{
    public MccCommandHandler(GameSettings gameSettings,
        IRepository companyRepository) 
        : base("mcc", gameSettings, companyRepository)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        PublishMessage(@"Instructions for MegaCorpClash are at https://megacorpclash.com/game-instructions/");
    }
}