using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestStatusCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompany()
    {
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "status", "");

        var commandHandler =
            new StatusCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.YouDoNotHaveACompany);
    }

    [Fact]
    public void Test_Success_DefaultMotto()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "status", "");

        var commandHandler =
            new StatusCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("At Player1Co we always say 'We don't need a motto'. That's how we earned 1,000 CorpoBux");
    }
}