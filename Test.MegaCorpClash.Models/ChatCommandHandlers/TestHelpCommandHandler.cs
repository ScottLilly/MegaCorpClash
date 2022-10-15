using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;
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
        //var chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal("", chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal("MegaCorpClash commands: !companies, !help, !hire, !incorporate, !jobs, !rename, !setmotto, !staff, !status, !steal",
        //    chatMessageEvent.Arguments.Message);

        //// Call a second time. Should have list created this time.
        //// This is for testing code coverage.
        //chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal("", chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal("MegaCorpClash commands: !companies, !help, !hire, !incorporate, !jobs, !rename, !setmotto, !staff, !status, !steal",
        //    chatMessageEvent.Arguments.Message);
    }
}