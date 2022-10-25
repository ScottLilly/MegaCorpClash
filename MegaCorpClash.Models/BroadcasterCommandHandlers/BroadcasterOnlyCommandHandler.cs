using MegaCorpClash.Models.ChatCommandHandlers;

namespace MegaCorpClash.Models.BroadcasterCommandHandlers;

public abstract class BroadcasterOnlyCommandHandler : BaseCommandHandler
{
    protected BroadcasterOnlyCommandHandler(
        string commandName, GameSettings gameSettings, Dictionary<string, Company> companies) 
        : base(commandName, gameSettings, companies)
    {
    }
}
