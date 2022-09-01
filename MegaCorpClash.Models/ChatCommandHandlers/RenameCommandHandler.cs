using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class RenameCommandHandler : BaseCommandHandler
{
    public RenameCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("rename", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        string chatterDisplayName = chatCommand.ChatterDisplayName();
        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (player == null)
        {
            PublishMessage(chatterDisplayName, 
                "You don't have a company. Type !incorporate <company name> to start one.");

            return;
        }

        string newCompanyName = chatCommand.ArgumentsAsString;

        if (string.IsNullOrWhiteSpace(newCompanyName))
        {
            PublishMessage(chatterDisplayName,
                "You must provide a new name for your company");

            return;
        }

        if (Players.Values.Any(p => p.CompanyName.Matches(newCompanyName)))
        {
            PublishMessage(chatterDisplayName, 
                $"There is already a company named {newCompanyName}");

            return;
        }

        player.CompanyName = newCompanyName;

        NotifyPlayerDataUpdated();

        PublishMessage(chatterDisplayName,
            $"Your company is now named {player.CompanyName}");
    }
}