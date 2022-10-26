using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class JobsCommandHandler : BaseCommandHandler
{
    public JobsCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("jobs", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        PublishMessage($"Jobs and cost to hire: {GameSettings.JobsList}");
    }
}