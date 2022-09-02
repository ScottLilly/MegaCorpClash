using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HelpCommandHandler : BaseCommandHandler
{
    private string _commandsAvailable = string.Empty;

    public HelpCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> players)
        : base("help", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        PopulateCommandList();

        PublishMessage($"MegaCorpClash commands: {_commandsAvailable}");
    }

    private void PopulateCommandList()
    {
        // Don't move this into the constructor.
        // It would get recursive and cause a stack overflow.
        if (!string.IsNullOrWhiteSpace(_commandsAvailable))
        {
            return;
        }

        var baseType = typeof(BaseCommandHandler);
        var assembly = baseType.Assembly;

        List<BaseCommandHandler> commandHandlers =
            assembly.GetTypes()
                .Where(t => t.IsSubclassOf(baseType))
                .Select(t => Activator.CreateInstance(t, GameSettings, Players))
                .Cast<BaseCommandHandler>()
                .ToList();

        _commandsAvailable =
            string.Join(", ", commandHandlers.Select(ch => $"!{ch.CommandName}")
                .OrderBy(cn => cn));
    }
}