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
        var chatter = ChatterDetails(chatCommand);

        PublishMessage(chatter.Name,
            chatter.Player == null
                ? Literals.YouDoNotHaveACompany
                : $"At {chatter.Player.CompanyName} we always say '{chatter.Player.Motto}'. That's how we earned {chatter.Player.Points} {GameSettings.PointsName}");
    }
}