using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class MccCommandHandler : BaseCommandHandler
{
    public MccCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs) 
        : base("mcc", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        PublishMessage(@"Instructions for MegaCorpClash are at https://megacorpclash.com/game-instructions/");
    }
}