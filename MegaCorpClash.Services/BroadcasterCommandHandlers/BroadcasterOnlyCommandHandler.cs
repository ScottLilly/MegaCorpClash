using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.BroadcasterCommandHandlers;

public abstract class BroadcasterOnlyCommandHandler : BaseCommandHandler
{
    protected BroadcasterOnlyCommandHandler(
        string commandName, GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base(commandName, gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }
}
