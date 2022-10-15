using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;

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

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Companies_NoCompaniesInGame, commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_OneCompany()
    {
        Dictionary<string, Company> companies = new();

        companies.Add("123", new Company
        {
            DisplayName = "Joe", 
            CompanyName = "JoeCo",
            Points = 99
        });

        var commandHandler =
            new CompaniesCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!companies");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Richest companies: JoeCo [99]", commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_TwoCompanies()
    {
        Dictionary<string, Company> companies = new();

        companies.Add("123", new Company
        {
            DisplayName = "Joe",
            CompanyName = "JoeCo",
            Points = 1111
        });

        companies.Add("456", new Company
        {
            DisplayName = "Sue",
            CompanyName = "SueCo",
            Points = 2222
        });

        var commandHandler =
            new CompaniesCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!companies");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Richest companies: SueCo [2,222], JoeCo [1,111]", commandHandler.ChatMessages.First());
    }
}