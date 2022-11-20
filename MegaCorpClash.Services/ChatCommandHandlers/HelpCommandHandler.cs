using MegaCorpClash.Models;
using MegaCorpClash.Services.BroadcasterCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class HelpCommandHandler : BaseCommandHandler
{
    private static string s_commandsAvailable = string.Empty;
    private static readonly object s_syncLock = new();

    public HelpCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base("help", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

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
                        .Select(t => Activator.CreateInstance(t, GameSettings, CompanyCompanyRepository, null))
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