using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class JobsCommandHandler : BaseCommandHandler
{
    public JobsCommandHandler(GameSettings gameSettings,
        IRepository companyRepository)
        : base("jobs", gameSettings, companyRepository)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

        PublishMessage($"Jobs and cost to hire: {GameSettings.JobsList}");
    }
}