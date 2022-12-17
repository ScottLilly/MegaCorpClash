using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Models;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestStartCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompanyNamePassed()
    {
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "start", "");

        var commandHandler =
            new StartCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Start_NameRequired);
    }

    [Fact]
    public void Test_CompanyNameNotSafeText()
    {
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "start", "1{a");

        var commandHandler =
            new StartCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Start_NotSafeText);
    }

    [Fact]
    public void Test_CompanyNameTooLong()
    {
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "start", 
            "123456789012345678901");

        var commandHandler =
            new StartCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Company name cannot be longer than 20 characters");
    }

    [Fact]
    public void Test_AlreadyHasACompany()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "start", "asd");

        var commandHandler =
            new StartCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You already have a company named Player1Co");
    }

    [Fact]
    public void Test_CompanyNameAlreadyUsed()
    {
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "start", "Player2Co");

        var commandHandler =
            new StartCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("There is already a company named Player2Co");
    }

    [Fact]
    public void Test_Success()
    {
        // TODO
        //var repo = GetTestInMemoryRepository();
        //var args = GetGameCommandArgs(new Company(), "start", "UniqueNameCo");

        //var commandHandler =
        //    new StartCommandHandler(GetDefaultGameSettings(), repo, args);

        //commandHandler.Execute();

        //commandHandler.ChatMessages.First()
        //    .ShouldBe("You are now the proud CEO of UniqueNameCo");
    }
}