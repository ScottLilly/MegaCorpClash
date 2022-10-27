using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestJobsCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    public TestJobsCommandHandler()
    {
        _gameSettings.EmployeeHiringDetails.Add(new GameSettings.EmployeeHiringInfo
        {
            Type = EmployeeType.Sales,
            CostToHire = 10
        });

        _gameSettings.EmployeeHiringDetails.Add(new GameSettings.EmployeeHiringInfo
        {
            Type = EmployeeType.Marketing,
            CostToHire = 25
        });
    }

    [Fact]
    public void Test_JobsList()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new JobsCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!jobs");

        commandHandler.Execute(gameCommand);

        Assert.Equal("Jobs and cost to hire: Marketing [25], Sales [10]",
            commandHandler.ChatMessages.First());
    }
}