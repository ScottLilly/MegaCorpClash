using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class JobsCommandHandler : BaseCommandHandler
{
    public JobsCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("jobs", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommand)
    {
        PublishMessage($"Jobs and cost to hire: {GameSettings.JobsList}");
    }
}