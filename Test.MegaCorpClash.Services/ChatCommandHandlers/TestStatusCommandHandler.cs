using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

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

        var gameCommand = GetGameCommandArgs("!status");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.YouDoNotHaveACompany,
            commandHandler.ChatMessages.First());
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

        var gameCommand = GetGameCommandArgs("!status");

        commandHandler.Execute(gameCommand);

        Assert.Equal("At ScottCo we always say 'We don't need a motto'. That's how we earned 100 CorpoBux",
            commandHandler.ChatMessages.First());
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

        var gameCommand = GetGameCommandArgs("!status");

        commandHandler.Execute(gameCommand);

        Assert.Equal("At ScottCo we always say 'This is our new motto'. That's how we earned 1,000,000 CorpoBux",
            commandHandler.ChatMessages.First());
    }
}