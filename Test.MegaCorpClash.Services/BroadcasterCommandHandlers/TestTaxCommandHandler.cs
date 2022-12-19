using MegaCorpClash.Services.BroadcasterCommandHandlers;
using Shouldly;
using Test.MegaCorpClash.Services.ChatCommandHandlers;

namespace Test.MegaCorpClash.Services.BroadcasterCommandHandlers;

public class TestTaxCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NotBroadcaster()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "tax", "1000");

        var commandHandler =
            new TaxCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.ShouldBeEmpty();
    }

    [Fact]
    public void Test_TaxBadParameter_NegativeNumber()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetBroadcasterCompany();
        var args = GetGameCommandArgs(company, "tax", "-1");

        var commandHandler =
            new TaxCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Tax command requires a single integer tax rate between 1 and 99");
    }

    [Fact]
    public void Test_TaxBadParameter_Zero()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetBroadcasterCompany();
        var args = GetGameCommandArgs(company, "tax", "0");

        var commandHandler =
            new TaxCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Tax command requires a single integer tax rate between 1 and 99");
    }

    [Fact]
    public void Test_TaxBadParameter_GreaterThan99()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetBroadcasterCompany();
        var args = GetGameCommandArgs(company, "tax", "100");

        var commandHandler =
            new TaxCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Tax command requires a single integer tax rate between 1 and 99");
    }

    [Fact]
    public void Test_TaxValid()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetBroadcasterCompany();
        var args = GetGameCommandArgs(company, "tax", "10");

        var commandHandler =
            new TaxCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("A 10% tax was applied to all companies");
    }
}
