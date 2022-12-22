using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestJobsCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_JobsList()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "jobs", "");

        var commandHandler =
            new JobsCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Jobs and cost to hire: HR [40], IT [100], Legal [150], Marketing [75], PR [125], Production [25], Research [250], Sales [50], Security [100], Spy [500]");
    }
}