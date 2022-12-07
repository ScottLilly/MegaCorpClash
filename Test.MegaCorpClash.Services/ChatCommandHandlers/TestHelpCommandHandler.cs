using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestHelpCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_HelpMessage()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "help", "");

        var commandHandler =
            new HelpCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("MegaCorpClash commands: !companies, !help, !hire, !jobs, !mcc, !motto, !recruit, !rename, !staff, !start, !status, !steal, !strike");

        // Call a second time. Should have list created this time.
        // This is for testing code coverage.
        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("MegaCorpClash commands: !companies, !help, !hire, !jobs, !mcc, !motto, !recruit, !rename, !staff, !start, !status, !steal, !strike");
    }
}