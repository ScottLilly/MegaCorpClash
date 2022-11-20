using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class JobsCommandHandler : BaseCommandHandler
{
    public JobsCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base("jobs", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        PublishMessage($"Jobs and cost to hire: {GameSettings.JobsList}");
    }
}