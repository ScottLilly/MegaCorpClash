using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HelpCommandHandler : BaseCommandHandler
{
    public HelpCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("help", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        PublishMessage(chatCommand.ChatterDisplayName(),
            "Available MegaCorpClash commands: !incorporate, !companies, !employees, !rename, !setmotto, !status, and !help");
    }
}