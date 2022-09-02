using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestIncorporateCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings = 
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompanyNamePassed()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, companies);

        var chatCommand = GetChatCommand("!incorporate");

        var chatMessageEvent = 
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(chatCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME, 
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.Incorporate_NameRequired, 
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_AlreadyHasACompany()
    {
        Dictionary<string, Company> playerList = new();
        playerList.Add(DEFAULT_CHATTER_ID,
            new Company { CompanyName = "Test" });

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, playerList);

        var chatCommand = GetChatCommand("!incorporate ABC");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(chatCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("You already have a company named Test",
            chatMessageEvent.Arguments.Message);
    }
}