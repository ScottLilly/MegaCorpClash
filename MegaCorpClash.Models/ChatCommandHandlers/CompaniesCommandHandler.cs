using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class CompaniesCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    private string PlayerList =>
        string.Join(", ", 
            _players.Values
                .OrderBy(c => c.CompanyName)
                .Select(c => $"{c.CompanyName} ({c.DisplayName})"));

    public CompaniesCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("companies", gameSettings, players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        PublishMessage(chatCommand.ChatterDisplayName(), $"Companies: {PlayerList}");
    }
}