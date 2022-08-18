using CSharpExtender.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class IncorporateCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public string CommandText => "incorporate";

    public IncorporateCommandHandler(Dictionary<string, Player> players)
        : base(players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        if (!chatCommand.ArgumentsAsList.Any())
        {
            InvokeMessageToDisplay(chatCommand, 
                $"{DisplayName(chatCommand)} - !incorporate must be followed by a name for your company");

            return;
        }

        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (player != null)
        {
            InvokeMessageToDisplay(chatCommand,
                $"{DisplayName(chatCommand)}, you already have a company name {player.CompanyName}");

            return;
        }

        if (_players.Values.Any(p => p.CompanyName.Matches(Arguments(chatCommand))))
        {
            InvokeMessageToDisplay(chatCommand,
                $"{DisplayName(chatCommand)}, there is already a company named {Arguments(chatCommand)}");

            return;
        }

        player = new Player
        {
            Id = TwitchUserId(chatCommand),
            DisplayName = DisplayName(chatCommand),
            CompanyName = Arguments(chatCommand),
            CreatedOn = DateTime.UtcNow
        };

        _players[TwitchUserId(chatCommand)] = player;

        NotifyPlayerDataUpdated();

        InvokeMessageToDisplay(chatCommand,
            $"{DisplayName(chatCommand)}, you are now the proud CEO of {player.CompanyName}");
    }
}