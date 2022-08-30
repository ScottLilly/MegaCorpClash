using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class SetMottoCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public SetMottoCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("setmotto", gameSettings, players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        string chatterDisplayName = chatCommand.ChatterDisplayName();
        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (string.IsNullOrWhiteSpace(chatCommand.ArgumentsAsString))
        {
            PublishMessage(chatterDisplayName, "You must enter a value for the motto");
            return;
        }

        if (player == null)
        {
            PublishMessage(chatterDisplayName, "You do not have a company");
            return;
        }

        player.Motto = chatCommand.ArgumentsAsString;
        NotifyPlayerDataUpdated();
        PublishMessage(chatterDisplayName,
                $"Your new company motto is '{player.Motto}'");
    }
}