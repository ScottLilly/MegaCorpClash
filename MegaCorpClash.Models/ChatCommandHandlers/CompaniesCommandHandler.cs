using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class CompaniesCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public string CommandText => "companies";

    public CompaniesCommandHandler(Dictionary<string, Player> players)
        : base(players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        InvokeMessageToDisplay(chatCommand, 
            $"Companies: {string.Join(", ", _players.Values.OrderBy(c => c.CompanyName).Select(c => $"{c.CompanyName} ({c.DisplayName})"))}");
    }
}