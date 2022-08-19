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
        string chatterDisplayName = DisplayName(chatCommand);
        Player? player = GetPlayerObjectForChatter(chatCommand);

        PublishMessage(chatterDisplayName,
            player == null
                ? "You do not have a company"
                : $"Your company {player.CompanyName} has {player.Points} {_pointsName}");
    }
}