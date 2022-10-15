using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestHelpCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_HelpMessage()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new HelpCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!help");

        // Call the first time. List needs to be created.
        commandHandler.Execute(gameCommand);

        Assert.Equal("MegaCorpClash commands: !companies, !help, !hire, !incorporate, !jobs, !rename, !setmotto, !staff, !status, !steal",
            commandHandler.ChatMessages.First());

        //// Call a second time. Should have list created this time.
        //// This is for testing code coverage.
        commandHandler.Execute(gameCommand);

        Assert.Equal("MegaCorpClash commands: !companies, !help, !hire, !incorporate, !jobs, !rename, !setmotto, !staff, !status, !steal",
            commandHandler.ChatMessages.First());
    }
}