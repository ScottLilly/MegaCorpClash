using MegaCorpClash.Models.BroadcasterCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class HelpCommandHandler : BaseCommandHandler
{
    private static string s_commandsAvailable = string.Empty;
    private static readonly object s_syncLock = new();

    public HelpCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("help", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        PublishMessage($"MegaCorpClash commands: {GetGameCommands()}");
    }

    private string GetGameCommands()
    {
        lock (s_syncLock)
        {
            if (string.IsNullOrWhiteSpace(s_commandsAvailable))
            {
                var baseType = typeof(BaseCommandHandler);
                var assembly = baseType.Assembly;

                List<BaseCommandHandler> commandHandlers =
                    assembly.GetTypes()
                        .Where(t => t.IsSubclassOf(baseType) &&
                        !t.IsAbstract &&
                        !t.IsSubclassOf(typeof(BroadcasterOnlyCommandHandler)))
                        .Select(t => Activator.CreateInstance(t, GameSettings, Companies))
                        .Cast<BaseCommandHandler>()
                        .ToList();

                s_commandsAvailable =
                    string.Join(", ",
                        commandHandlers
                            .Select(ch => $"!{ch.CommandName}")
                            .OrderBy(cn => cn));
            }
        }

        return s_commandsAvailable;
    }
}