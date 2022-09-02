using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HelpCommandHandler : BaseCommandHandler
{
    private static string s_commandsAvailable = string.Empty;
    private static readonly object s_syncLock = new();

    public HelpCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("help", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        PopulateCommandList();

        PublishMessage($"MegaCorpClash commands: {s_commandsAvailable}");
    }

    private void PopulateCommandList()
    {
        // Don't move this into the constructor.
        // It would get recursive and cause a stack overflow.
        lock (s_syncLock)
        {
            if (!string.IsNullOrWhiteSpace(s_commandsAvailable))
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

            s_commandsAvailable =
                string.Join(", ", commandHandlers.Select(ch => $"!{ch.CommandName}")
                    .OrderBy(cn => cn));
        }
    }
}