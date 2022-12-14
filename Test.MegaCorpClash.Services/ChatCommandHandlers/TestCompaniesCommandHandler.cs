using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestCompaniesCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompanies()
    {
        // Setup
        var repo = GetTestInMemoryRepository(false, 0);
        var args = GetGameCommandArgs(new Company(), "companies", "");

        var commandHandler =
            new CompaniesCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Companies_NoCompaniesInGame);
    }

    [Fact]
    public void Test_OneCompany()
    {
        // Setup
        var repo = GetTestInMemoryRepository(true, 0);
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "companies", "");

        var commandHandler =
            new CompaniesCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Richest companies: ScottCo [1,000,000]");
    }

    [Fact]
    public void Test_TwoCompanies()
    {
        // Setup
        var repo = GetTestInMemoryRepository(true, 1);
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "companies", "");

        var commandHandler =
            new CompaniesCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Richest companies: ScottCo [1,000,000], Player1Co [1,000]");
    }
}