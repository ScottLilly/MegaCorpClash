﻿using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Models;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestStartCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompanyNamePassed()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StartCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!start");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Start_NameRequired,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyNameNotSafeText()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StartCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!start 1{a");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Start_NotSafeText,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyNameTooLong()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StartCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!start 1234567890123456");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Company name cannot be longer than 15 characters",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_AlreadyHasACompany()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company { CompanyName = "Test" });

        var commandHandler =
            new StartCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!start ABC");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You already have a company named Test",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyNameAlreadyUsed()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("999",
            new Company { CompanyName = "Test" });

        var commandHandler =
            new StartCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!start Test");

        commandHandler.Execute(gameCommand);

        Assert.Equal("There is already a company named Test",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_Success()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StartCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!start Test");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You are now the proud CEO of Test",
            commandHandler.ChatMessages.First());

        Company company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal(2, company.Employees.Count);
    }
}