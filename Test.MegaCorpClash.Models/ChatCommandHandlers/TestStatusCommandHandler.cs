using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestStatusCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StatusCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!status");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessageToSend += h,
                h => commandHandler.OnChatMessageToSend -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.DisplayName);
        Assert.Equal(Literals.YouDoNotHaveACompany,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_Success_DefaultMotto()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new StatusCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!status");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessageToSend += h,
                h => commandHandler.OnChatMessageToSend -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.DisplayName);
        Assert.Equal("At ScottCo we always say 'We don't need a motto'. That's how we earned 100 CorpoBux",
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_Success_ModifiedMotto()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Motto = "This is our new motto",
                Points = 1000000
            });

        var commandHandler =
            new StatusCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!status");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessageToSend += h,
                h => commandHandler.OnChatMessageToSend -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.DisplayName);
        Assert.Equal("At ScottCo we always say 'This is our new motto'. That's how we earned 1,000,000 CorpoBux",
            chatMessageEvent.Arguments.Message);
    }
}