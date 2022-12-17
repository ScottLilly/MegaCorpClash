using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestRenameCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompany()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "rename", "");

        var commandHandler =
            new RenameCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.YouDoNotHaveACompany);
    }

    [Fact]
    public void Test_NoCompanyNamePassed()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "rename", "");

        var commandHandler =
            new RenameCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Rename_YouMustProvideANewName);
    }

    [Fact]
    public void Test_CompanyName_InvalidCharacters()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "rename", "u[k");

        var commandHandler =
            new RenameCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Start_NotSafeText);
    }

    [Fact]
    public void Test_CompanyName_NameTooLong()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "rename", "123456789012345678901");

        var commandHandler =
            new RenameCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Company name cannot be longer than 20 characters");
    }

    [Fact]
    public void Test_CompanyNameAlreadyUsed()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "rename", "ScottCo");

        var commandHandler =
            new RenameCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("There is already a company named ScottCo");
    }

    [Fact]
    public void Test_Success()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "rename", "SecondCo");

        var commandHandler =
            new RenameCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Your company is now named SecondCo");
    }
}