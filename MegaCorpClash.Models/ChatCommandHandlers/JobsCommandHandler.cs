using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class JobsCommandHandler : BaseCommandHandler
{
    private readonly string _jobList;

    public JobsCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("jobs", gameSettings, players)
    {
        _jobList =
            string.Join(", ",
            GameSettings
            .EmployeeHiringDetails
            .OrderBy(e => e.Type)
            .Select(e => $"{e.Type} ({e.CostToHire})"));
    }

    public override void Execute(ChatCommand chatCommand)
    {
        PublishMessage($"Jobs and cost to hire: {_jobList}");
    }
}