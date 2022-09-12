namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class HelpCommandHandler : BaseCommandHandler
{
    private string _commandsAvailable = string.Empty;

    public HelpCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("help", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
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
                .Select(t => Activator.CreateInstance(t, GameSettings, Companies))
                .Cast<BaseCommandHandler>()
                .ToList();

        _commandsAvailable =
            string.Join(", ", commandHandlers.Select(ch => $"!{ch.CommandName}")
                .OrderBy(cn => cn));
    }
}