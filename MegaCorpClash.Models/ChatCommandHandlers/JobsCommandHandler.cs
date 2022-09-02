using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class JobsCommandHandler : BaseCommandHandler
{
    public JobsCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("jobs", gameSettings, companies)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        PublishMessage($"Jobs and cost to hire: {GameSettings.JobsList}");
    }
}