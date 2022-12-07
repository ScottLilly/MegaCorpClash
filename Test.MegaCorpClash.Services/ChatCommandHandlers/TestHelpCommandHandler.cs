using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestHelpCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_HelpMessage()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "help", "");

        var commandHandler =
            new HelpCommandHandler(_gameSettings, repo, args);

        commandHandler.Execute();

        Assert.Equal("MegaCorpClash commands: !companies, !help, !hire, !jobs, !mcc, !motto, !recruit, !rename, !staff, !start, !status, !steal, !strike",
            commandHandler.ChatMessages.First());

        // Call a second time. Should have list created this time.
        // This is for testing code coverage.
        commandHandler.Execute();

        Assert.Equal("MegaCorpClash commands: !companies, !help, !hire, !jobs, !mcc, !motto, !recruit, !rename, !staff, !start, !status, !steal, !strike",
            commandHandler.ChatMessages.First());
    }
}