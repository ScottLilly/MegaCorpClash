using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestStealCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new StealCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!steal");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.YouDoNotHaveACompany,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_NoSpy()
    {
        var commandHandler =
            GetStealCommandHandler(100, 0);

        var gameCommand = GetGameCommandArgs("!steal");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You must have at least one spy to steal",
            commandHandler.ChatMessages.First());
    }

    private StealCommandHandler GetStealCommandHandler(int points, int hiredSpies)
    {
        Dictionary<string, Company> companies = new();

        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = points
            });

        if (hiredSpies > 0)
        {
            companies[DEFAULT_CHATTER_ID].Employees = 
                new List<EmployeeQuantity> {
                    new()
                    {
                        Type = EmployeeType.Spy,
                        Quantity = hiredSpies
                    }};
        }

        return new StealCommandHandler(_gameSettings, companies);
    }
}