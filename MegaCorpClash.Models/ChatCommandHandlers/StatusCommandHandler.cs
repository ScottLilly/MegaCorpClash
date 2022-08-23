using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class StatusCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    private readonly string _pointsName;

    public string CommandName => "status";

    public StatusCommandHandler(Dictionary<string, Player> players, string pointsName)
        : base(players)
    {
        _pointsName = pointsName;
    }

    public void Execute(ChatCommand chatCommand)
    {
        string chatterDisplayName = chatCommand.ChatterDisplayName();
        Player? player = GetPlayerObjectForChatter(chatCommand);

        PublishMessage(chatterDisplayName,
            player == null
                ? "You do not have a company"
                : $"At {player.CompanyName} we always say '{player.Motto}'. That's how we earned {player.Points} {_pointsName}");
    }
}