using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;

namespace Test.MegaCorpClash.Models.ChatCommandHandlers;

public class TestHireCommandHandler : BaseCommandHandlerTest
{
    private readonly GameSettings _gameSettings =
        GetDefaultGameSettings();

    public TestHireCommandHandler()
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

    [Theory]
    [MemberData(nameof(InvalidParameterTestValues))]
    public void Test_InvalidParameters(string parameter)
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler();

        var gameCommand = GetGameCommandArgs($"!hire {parameter}");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Hire_InvalidParameters,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.YouDoNotHaveACompany,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_OneValidOneInvalid()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler(100);

        var gameCommand = GetGameCommandArgs("!hire Sales gibberish");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You hired 1 Sales employee and have 90 CorpoBux remaining.",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_OneValidTextParameter()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler(100);

        var gameCommand = GetGameCommandArgs("!hire sales");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You hired 1 Sales employee and have 90 CorpoBux remaining.",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_ZeroQuantity()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler();

        var gameCommand = GetGameCommandArgs("!hire Sales 0");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Hire_QuantityMustBeGreaterThanZero,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_NegativeQuantity()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler();

        var gameCommand = GetGameCommandArgs("!hire Sales -1");

        commandHandler.Execute(gameCommand);

        Assert.Equal(Literals.Hire_QuantityMustBeGreaterThanZero,
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_ValidParametersInsufficientMoney()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 9
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire Sales 1");

        commandHandler.Execute(gameCommand);

        Assert.Equal("It costs 10 CorpoBux to hire 1 Sales employees. You only have 9 CorpoBux",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_ValidParametersInsufficientMoneyToHireMax()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 9
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire Sales max");

        commandHandler.Execute(gameCommand);

        Assert.Equal("It costs 10 CorpoBux to hire a Sales employee. You only have 9 CorpoBux",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_ValidParametersMaxHireSufficientMoney()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 50
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire Sales max");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You hired 5 Sales employees and have 0 CorpoBux remaining.",
            commandHandler.ChatMessages.First());
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_OneEmployee()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 11
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire Sales 1");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You hired 1 Sales employee and have 1 CorpoBux remaining.",
            commandHandler.ChatMessages.First());

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Single(company.Employees);
        Assert.Equal(1, company.Points);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_OneEmployee_WithHRDiscount()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 11,
                Employees = new List<EmployeeQuantity>
                {
                    new()
                    {
                        Type = EmployeeType.HR, 
                        Quantity = 10
                    }
                }
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire Sales 1");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You hired 1 Sales employee and have 2 CorpoBux remaining.",
            commandHandler.ChatMessages.First());

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal(2, company.Employees.Count);
        Assert.Equal(2, company.Points);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_TwoEmployees()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 22
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommandArgs("!hire 2 sales");

        commandHandler.Execute(gameCommand);

        Assert.Equal("You hired 2 Sales employees and have 2 CorpoBux remaining.",
            commandHandler.ChatMessages.First());

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal(2, company.Employees.First(e => e.Type == EmployeeType.Sales).Quantity);
        Assert.Equal(2, company.Points);
    }

    public static IEnumerable<object[]> InvalidParameterTestValues()
    {
        yield return new object[] { "" };
        yield return new object[] { "1" };
        yield return new object[] { "asd" };
        yield return new object[] { "asd qwe" };
        yield return new object[] { "Sales Production" };
        yield return new object[] { "1 2 3" };
        yield return new object[] { "CEO 1" };
    }

    private HireCommandHandler GetHireCommandHandler(int points = 0)
    {
        Dictionary<string, Company> companies = new();

        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = points
            });

        return new HireCommandHandler(_gameSettings, companies);
    }
}