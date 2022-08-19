using CSharpExtender.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class RenameCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public string CommandName => "rename";

    public RenameCommandHandler(Dictionary<string, Player> players) 
        : base(players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (player == null)
        {
            PublishMessage(chatCommand, 
                $"{DisplayName(chatCommand)}, you don't have a company. Type !incorporate <company name> to start one.");
        }
        else
        {
            if (_players.Values.Any(p => p.CompanyName.Matches(Arguments(chatCommand))))
            {
                PublishMessage(chatCommand, 
                    $"{DisplayName(chatCommand)}, there is already a company named {Arguments(chatCommand)}");

                return;
            }

            player.CompanyName = Arguments(chatCommand);

            NotifyPlayerDataUpdated();
        }
    }
}