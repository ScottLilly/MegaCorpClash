﻿using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestStaffCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StaffCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!staff");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.YouDoNotHaveACompany,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_Success()
    {
        Dictionary<string, Company> companies = new();
        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                UserId = DEFAULT_CHATTER_ID,
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = "ScottCo",
                Points = 100,
                Employees = new List<EmployeeQuantity>
                {
                    new()
                    {
                        Type = EmployeeType.Production,
                        Quantity = 1
                    },
                    new()
                    {
                        Type = EmployeeType.Sales,
                        Quantity = 2
                    }
                }
            });

        var commandHandler =
            new StaffCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!staff");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You have 3 employees. 1 Production, 2 Sales",
            commandHandler.ChatMessages.First());
    }
}