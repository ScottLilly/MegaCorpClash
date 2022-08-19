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
        Player? player = GetPlayerObjectForChatter(chatCommand);

        PublishMessage(chatCommand,
            player == null
                ? $"{DisplayName(chatCommand)}, you do not have a company"
                : $"{DisplayName(chatCommand)}: Your company {player.CompanyName} has {player.Points} {_pointsName}");
    }
}