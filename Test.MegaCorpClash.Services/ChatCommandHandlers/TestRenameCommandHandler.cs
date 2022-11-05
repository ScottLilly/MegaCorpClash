using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

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

        var gameCommand = GetGameCommandArgs("!rename");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.YouDoNotHaveACompany,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_NoCompanyNamePassed()
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
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!rename");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Rename_YouMustProvideANewName,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyNameTooLong()
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
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!rename u[k");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Start_NotSafeText,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyNameNotSafeText()
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
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!rename 1234567890123456");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Company name cannot be longer than 15 characters",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyNameAlreadyUsed()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("999",
            new Company
            {
                UserId = "999",
                DisplayName = "OtherPlayer",
                CompanyName = "FirstCo",
                Points = 100
            });
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!rename FirstCo");

        commandHandler.Execute(gameCommand);

        Assert.Equal("There is already a company named FirstCo",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_Success()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("999",
            new Company
            {
                UserId = "999",
                DisplayName = "OtherPlayer",
                CompanyName = "FirstCo",
                Points = 100
            });
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new RenameCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!rename SecondCo");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Your company is now named SecondCo",
            commandHandler.ChatMessages.First());
    }
}