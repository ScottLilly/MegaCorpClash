using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestCompaniesCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompanies()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new CompaniesCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!companies");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal("", chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.Companies_NoCompaniesInGame, chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_OneCompany()
    {
        Dictionary<string, Company> companies = new();

        companies.Add("123", new Company
        {
            ChatterName = "Joe", 
            CompanyName = "JoeCo",
            Points = 99
        });

        var commandHandler =
            new CompaniesCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!companies");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal("", chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("Top companies: JoeCo: 99", chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_TwoCompanies()
    {
        Dictionary<string, Company> companies = new();

        companies.Add("123", new Company
        {
            ChatterName = "Joe",
            CompanyName = "JoeCo",
            Points = 1111
        });

        companies.Add("456", new Company
        {
            ChatterName = "Sue",
            CompanyName = "SueCo",
            Points = 2222
        });

        var commandHandler =
            new CompaniesCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!companies");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal("", chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("Top companies: SueCo: 2,222, JoeCo: 1,111", chatMessageEvent.Arguments.Message);
    }
}