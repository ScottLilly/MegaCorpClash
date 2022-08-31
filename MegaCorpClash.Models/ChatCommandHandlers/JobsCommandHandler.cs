using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class JobsCommandHandler : BaseCommandHandler
{
    readonly string _jobList;

    public JobsCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("jobs", gameSettings, players)
    {
        _jobList =
            string.Join(", ",
            GameSettings
            .EmployeeHiringDetails
            .OrderBy(e => e.EmployeeType)
            .Select(e => $"{e.EmployeeType} ({e.CostToHire})"));
    }

    public override void Execute(ChatCommand chatCommand)
    {
        PublishMessage(chatCommand.ChatterDisplayName(),
            $"Job description and cost to hire: {_jobList}");
    }
}