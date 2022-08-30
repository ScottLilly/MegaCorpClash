using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class StatusCommandHandler : BaseCommandHandler
{
    public StatusCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("status", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        string chatterDisplayName = chatCommand.ChatterDisplayName();
        Player? player = GetPlayerObjectForChatter(chatCommand);

        PublishMessage(chatterDisplayName,
            player == null
                ? "You do not have a company"
                : $"At {player.CompanyName} we always say '{player.Motto}'. That's how we earned {player.Points} {GameSettings.PointsName}");
    }
}