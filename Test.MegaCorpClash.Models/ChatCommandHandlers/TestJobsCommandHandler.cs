using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

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

        var gameCommand = GetGameCommand("!jobs");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessageToSend += h,
                h => commandHandler.OnChatMessageToSend -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal("",
            chatMessageEvent.Arguments.DisplayName);
        Assert.Equal("Jobs and cost to hire: Marketing (25), Sales (10)",
            chatMessageEvent.Arguments.Message);
    }
}