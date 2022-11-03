using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace MegaCorpClash.Services.BroadcasterCommandHandlers;

public abstract class BroadcasterOnlyCommandHandler : BaseCommandHandler
{
    protected BroadcasterOnlyCommandHandler(
        string commandName, GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base(commandName, gameSettings, companies)
    {
    }
}
