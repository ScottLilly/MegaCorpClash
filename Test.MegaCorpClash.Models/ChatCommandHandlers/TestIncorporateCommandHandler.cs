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

        var gameCommand = GetGameCommand("!incorporate");

        //var chatMessageEvent = 
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME, 
        //    chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal(Literals.Incorporate_NameRequired, 
        //    chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_CompanyNameNotSafeText()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!incorporate 1{a");

        //var chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
        //    chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal(Literals.Incorporate_NotSafeText,
        //    chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_CompanyNameTooLong()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!incorporate 1234567890123456");

        //var chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
        //    chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal("Company name cannot be longer than 15 characters",
        //    chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_AlreadyHasACompany()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company { CompanyName = "Test" });

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!incorporate ABC");

        //var chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
        //    chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal("You already have a company named Test",
        //    chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_CompanyNameAlreadyUsed()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("999",
            new Company { CompanyName = "Test" });

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!incorporate Test");

        //var chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
        //    chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal("There is already a company named Test",
        //    chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_Success()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new IncorporateCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!incorporate Test");

        //var chatMessageEvent =
        //    Assert.Raises<ChatMessageEventArgs>(
        //        h => commandHandler.OnChatMessageToSend += h,
        //        h => commandHandler.OnChatMessageToSend -= h,
        //        () => commandHandler.Execute(gameCommand));

        //Assert.NotNull(chatMessageEvent);
        //Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
        //    chatMessageEvent.Arguments.DisplayName);
        //Assert.Equal("You are now the proud CEO of Test",
        //    chatMessageEvent.Arguments.Message);

        //Company company = companies[DEFAULT_CHATTER_ID];

        //Assert.Equal(2, company.Employees.Count);
    }
}