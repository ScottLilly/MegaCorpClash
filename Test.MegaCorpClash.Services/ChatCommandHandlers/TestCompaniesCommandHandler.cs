using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;
using Moq;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestCompaniesCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings = GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompanies()
    {
        var gameCommand = GetGameCommandArgs("!companies");
        var commandHandler = BuildService(_gameSettings, gameCommand, Array.Empty<Company>());

        commandHandler.Execute();
        Assert.Equal(Literals.Companies_NoCompaniesInGame, commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_OneCompany()
    {
        var gameCommand = GetGameCommandArgs("!companies");

        var expectedReturn = new[]
        {
            new Company
            {
                DisplayName = "Joe",
                CompanyName = "JoeCo",
                Points = 99
            }
        };
        var commandHandler = BuildService(_gameSettings, gameCommand, expectedReturn);
        commandHandler.Execute();

        Assert.Equal("Richest companies: JoeCo [99]", commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_TwoCompanies()
    {
        var gameCommand = GetGameCommandArgs("!companies");

        var expectedReturn = new[]
        {
            new Company
            {
                DisplayName = "Joe",
                CompanyName = "JoeCo",
                Points = 1111
            },
            new Company
            {
                DisplayName = "Sue",
                CompanyName = "SueCo",
                Points = 2222
            }
        };
        var commandHandler = BuildService(_gameSettings, gameCommand, expectedReturn);
        commandHandler.Execute();

        Assert.Equal("Richest companies: SueCo [2,222], JoeCo [1,111]", commandHandler.ChatMessages.First());
    }
    
    [Fact]
    public void Ensure_We_only_return_back_top_7_richest()
    {
        var gameCommand = GetGameCommandArgs("!companies");

        var expectedReturn = Enumerable.Range(1, 100).Select(e => new Company
        {
            DisplayName = e.ToString(),
            CompanyName = e.ToString(),
            Points = e
        });
        var commandHandler = BuildService(_gameSettings, gameCommand, expectedReturn);
        commandHandler.Execute();

        Assert.Equal(7, commandHandler.ChatMessages.First().Split(',').Length);
    }

    private static CompaniesCommandHandler BuildService(
        GameSettings gameSettings, GameCommandArgs gameCommandArgs, IEnumerable<Company> companies)
    {
        var (repo, commandHandler) = BuildService(gameSettings, gameCommandArgs);
        repo.Setup(e => e.GetAllCompanies()).Returns(companies);
        repo.Setup(s => s.GetRichestCompanies(It.IsAny<int>()))
            .Returns((int x) => companies.OrderByDescending(x => x.Points).Take(x));
        return commandHandler;
    }

    private static (Mock<ICompanyRepository> mockRepo, CompaniesCommandHandler companiesCommandHandler) BuildService(
        GameSettings gameSettings, GameCommandArgs gameCommandArgs)
    {
        var mockRepo = new Mock<ICompanyRepository>();
        var commandHandler = new CompaniesCommandHandler(gameSettings, mockRepo.Object, gameCommandArgs);
        return (mockRepo, commandHandler);
    }
}