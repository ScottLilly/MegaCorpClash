using MegaCorpClash.Models;
using MegaCorpClash.Services.BroadcasterCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using Test.MegaCorpClash.Models.ChatCommandHandlers;

namespace Test.MegaCorpClash.Models.BroadcasterCommandHandlers;

public class TestTaxCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NotBroadcaster()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("123321",
            new Company
            {
                DisplayName = "NOT_BROADCASTER",
                CompanyName = "NOT_BROADCASTER",
                Points = 100
            });

        var commandHandler =
            new TaxCommandHandler(_gameSettings, companies);

        var gameCommandArgs =
            new GameCommandArgs("123321", "NOT_BROADCASTER", "tax", "5", false, false, false, false);

        commandHandler.Execute(gameCommandArgs);

        Assert.Empty(commandHandler.ChatMessages);
        Assert.False(commandHandler.PlayerDataUpdated);
    }

    [Fact]
    public void Test_TaxBadParameter_NegativeNumber()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("123321",
            new Company
            {
                DisplayName = "BROADCASTER",
                CompanyName = "BROADCASTER",
                Points = 100
            });

        var commandHandler =
            new TaxCommandHandler(_gameSettings, companies);

        var gameCommandArgs =
            new GameCommandArgs("123321", "BROADCASTER", "tax", "-1", true, false, false, false);

        commandHandler.Execute(gameCommandArgs);

        Assert.Equal("Tax command requires a single integer tax rate between 1 and 99", commandHandler.ChatMessages.First());
        Assert.False(commandHandler.PlayerDataUpdated);
    }

    [Fact]
    public void Test_TaxBadParameter_Zero()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("123321",
            new Company
            {
                DisplayName = "BROADCASTER",
                CompanyName = "BROADCASTER",
                Points = 100
            });

        var commandHandler =
            new TaxCommandHandler(_gameSettings, companies);

        var gameCommandArgs =
            new GameCommandArgs("123321", "BROADCASTER", "tax", "0", true, false, false, false);

        commandHandler.Execute(gameCommandArgs);

        Assert.Equal("Tax command requires a single integer tax rate between 1 and 99", commandHandler.ChatMessages.First());
        Assert.False(commandHandler.PlayerDataUpdated);
    }

    [Fact]
    public void Test_TaxBadParameter_GreaterThan99()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("123321",
            new Company
            {
                DisplayName = "BROADCASTER",
                CompanyName = "BROADCASTER",
                Points = 100
            });

        var commandHandler =
            new TaxCommandHandler(_gameSettings, companies);

        var gameCommandArgs =
            new GameCommandArgs("123321", "BROADCASTER", "tax", "100", true, false, false, false);

        commandHandler.Execute(gameCommandArgs);

        Assert.Equal("Tax command requires a single integer tax rate between 1 and 99", commandHandler.ChatMessages.First());
        Assert.False(commandHandler.PlayerDataUpdated);
    }

    [Fact]
    public void Test_TaxValid()
    {
        Dictionary<string, Company> companies = new();
        companies.Add("123321",
            new Company
            {
                IsBroadcaster = true,
                DisplayName = "BROADCASTER",
                CompanyName = "BROADCASTER",
                Points = 100
            });
        companies.Add("123322",
            new Company
            {
                DisplayName = "CHATTER",
                CompanyName = "CHATTER",
                Points = 100
            });

        var commandHandler =
            new TaxCommandHandler(_gameSettings, companies);

        var gameCommandArgs =
            new GameCommandArgs("123321", "BROADCASTER", "tax", "10", true, false, false, false);

        commandHandler.Execute(gameCommandArgs);

        Assert.Equal("A 10% tax was applied to all companies", commandHandler.ChatMessages.First());
        Assert.True(commandHandler.PlayerDataUpdated);
        Assert.Equal(100, companies.First(c => c.Value.IsBroadcaster).Value.Points);
        Assert.Equal(90, companies.First(c => !c.Value.IsBroadcaster).Value.Points);
    }
}
