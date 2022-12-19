﻿using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestStrikeCommandHandler : BaseCommandHandlerTest
{
    [Fact]
    public void Test_NoCompany()
    {
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(new Company(), "strike", "");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.YouDoNotHaveACompany);
    }

    [Fact]
    public void Test_NegativeSpyValue()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "-1");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Number of attacking spies must be greater than 0");
    }

    [Fact]
    public void Test_ZeroSpyValue()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "0");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("Number of attacking spies must be greater than 0");
    }

    [Fact]
    public void Test_AlphaSpyValue_AssumesUseOneSpy()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "A");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You don't have any spies");
    }

    [Fact]
    public void Test_AlphaSpyValue_BlankUseOneSpy()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You don't have any spies");
    }

    [Fact]
    public void Test_AlphaSpyValue_ExplicitUseOneSpy()
    {
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "1");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You don't have any spies");
    }

    [Fact]
    public void Test_UseTwoSpies_OnlyHaveOne()
    {
        var repo = GetTestInMemoryRepository();
        repo.HireEmployees("101", EmployeeType.Spy, 1, 0);
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "2");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You only have 1 spy");
    }

    [Fact]
    public void Test_UseThreeSpies_OnlyHaveTwo()
    {
        var repo = GetTestInMemoryRepository();
        repo.HireEmployees("101", EmployeeType.Spy, 2, 0);
        var company = repo.GetCompany("101");
        var args = GetGameCommandArgs(company, "strike", "3");

        var commandHandler =
            new StrikeCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You only have 2 spies");
    }

}