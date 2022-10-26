﻿using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestMottoCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new MottoCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!setmotto");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.YouDoNotHaveACompany,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_HasCompanyMissingMottoParameter()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new MottoCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!setmotto");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You must enter a value for the motto",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyMottoNotSafeText()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new MottoCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!setmotto kjh*234");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.SetMotto_NotSafeText,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_CompanyMottoTooLong()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var commandHandler =
            new MottoCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!setmotto 12345678901234567890123456");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Motto cannot be longer than 25 characters",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_Success_DefaultMotto()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100
            });

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal("We don't need a motto", company.Motto);
    }

    [Fact]
    public void Test_Success_ModifiedMotto()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 99
            });

        var commandHandler =
            new MottoCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!setmotto This is our new motto");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Your new company motto is 'This is our new motto'",
            commandHandler.ChatMessages.First());
    }
}