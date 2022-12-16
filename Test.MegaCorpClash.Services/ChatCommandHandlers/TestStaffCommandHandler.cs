using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestStaffCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompany()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "staff", "");

        var commandHandler =
            new StaffCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.YouDoNotHaveACompany);
    }

    [Fact]
    public void Test_Success()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetCompany("101"), "staff", "");

        var commandHandler =
            new StaffCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You have 20 employees. 10 Production, 10 Sales");
    }
}