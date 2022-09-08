using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestRenameCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!rename");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.YouDoNotHaveACompany,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_NoCompanyNamePassed()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!rename");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.Rename_YouMustProvideANewName,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_CompanyNameNotSafeText()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!rename u[k");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.CompanyName_NotSafeText,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_CompanyNameAlreadyUsed()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("999",
            new Company
            {
                ChatterId = "999",
                ChatterName = "OtherPlayer",
                CompanyName = "FirstCo",
                Points = 100
            });
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!rename FirstCo");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("There is already a company named FirstCo",
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_Success()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("999",
            new Company
            {
                ChatterId = "999",
                ChatterName = "OtherPlayer",
                CompanyName = "FirstCo",
                Points = 100
            });
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterId = DEFAULT_CHATTER_ID,
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!rename SecondCo");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("Your company is now named SecondCo",
            chatMessageEvent.Arguments.Message);
    }
}