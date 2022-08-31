using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HelpCommandHandler : BaseCommandHandler
{
    private string _commandsAvailable = string.Empty;

    public HelpCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("help", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        if(string.IsNullOrWhiteSpace(_commandsAvailable))
        {
            var baseType = typeof(BaseCommandHandler);
            var assembly = baseType.Assembly;

            List<BaseCommandHandler> commandHandlers =
                assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseType))
                .Select(t => Activator.CreateInstance(t, new object[] { GameSettings, _players }))
                .Cast<BaseCommandHandler>()
                .ToList();

            _commandsAvailable = 
                string.Join(", ", commandHandlers.Select(ch => $"!{ch.CommandName}")
                .OrderBy(cn => cn));
        }

        PublishMessage(chatCommand.ChatterDisplayName(),
            $"MegaCorpClash commands: {_commandsAvailable}");
    }
}