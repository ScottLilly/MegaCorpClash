using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestMottoCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompany()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "motto", "");

        var commandHandler =
            new MottoCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.YouDoNotHaveACompany);
    }

    [Fact]
    public void Test_HasCompanyMissingMottoParameter()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetCompany("101"), "motto", "");

        var commandHandler =
            new MottoCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You must enter a value for the motto");
    }

    [Fact]
    public void Test_CompanyMottoNotSafeText()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetCompany("101"), "motto", "kjh*234");

        var commandHandler =
            new MottoCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.SetMotto_NotSafeText);
    }

    [Fact]
    public void Test_CompanyMottoTooLong()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetCompany("101"), 
            "motto", "12345678901234567890123456");

        var commandHandler =
            new MottoCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Motto cannot be longer than 25 characters");
    }

    [Fact]
    public void Test_Success_DefaultMotto()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        company.Motto.ShouldBe("We don't need a motto");
    }

    [Fact]
    public void Test_Success_ModifiedMotto()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetCompany("101"),
            "motto", "This is our new motto");

        var commandHandler =
            new MottoCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Your new company motto is 'This is our new motto'");

        var updatedCompany = repo.GetCompany("101");
        updatedCompany.Motto.ShouldBe("This is our new motto");
    }
}