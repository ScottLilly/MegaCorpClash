using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

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

    [Fact]
    public void Test_NoCompany()
    {
        Dictionary<string, Company> companies = new();

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!hire");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.YouDoNotHaveACompany,
            chatMessageEvent.Arguments.Message);
    }

    public record InvalidParameter(string Name, string Command)
    {
        private static readonly InvalidParameter[] TestCases =
        {
            new("NoParameters", ""),
            new("OneNumericParameter", "1"),
            new("OneInvalidTextParameter", "asd"),
            new("TwoTextParameters", "asd qwe"),
            new("TwoValidButNoQty", "Sales Production"),
            new("ThreeParameters", "1 2 3"),
            new("InvalidJobType", "CEO 1"),

        };

        public static IEnumerable<object[]> TestCasesData =>
            TestCases.Select(testCase => new object[] { testCase });

        public override string ToString() =>
            $"{Name} -- \"{Command}\"";
    }

    [Theory]
    [MemberData(nameof(InvalidParameter.TestCasesData), MemberType = typeof(InvalidParameter))]
    public void Test_InvalidParameter(InvalidParameter parameter)
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler();

        var gameCommand = GetGameCommand($"!hire {parameter.Command}");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.Hire_InvalidParameters,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_OneValidOneInvalid()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler(100);

        var gameCommand = GetGameCommand("!hire Sales gibberish");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("You hired 1 Sales employee.",
            chatMessageEvent.Arguments.Message);
    }
    [Fact]
    public void Test_OneValidTextParameter()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler(100);

        var gameCommand = GetGameCommand("!hire sales");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("You hired 1 Sales employee.",
            chatMessageEvent.Arguments.Message);
    }


    [Fact]
    public void Test_ZeroQuantity()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire Sales 0");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.Hire_QuantityMustBeGreaterThanZero,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_NegativeQuantity()
    {
        HireCommandHandler commandHandler =
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire Sales -1");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal(Literals.Hire_QuantityMustBeGreaterThanZero,
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_ValidParametersInsufficientMoney()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 9
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!hire Sales 1");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("It costs 10 CorpoBux to hire 1 Sales employees. You only have 9 CorpoBux",
            chatMessageEvent.Arguments.Message);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_OneEmployee()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 11
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!hire Sales 1");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("You hired 1 Sales employee.",
            chatMessageEvent.Arguments.Message);

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Single(company.Employees);
        Assert.Equal(1, company.Points);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_TwoEmployees()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(
            DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = 22
            });

        var commandHandler =
            new HireCommandHandler(_gameSettings, companies);

        var gameCommand = GetGameCommand("!hire 2 sales");

        var chatMessageEvent =
            Assert.Raises<ChatMessageEventArgs>(
                h => commandHandler.OnChatMessagePublished += h,
                h => commandHandler.OnChatMessagePublished -= h,
                () => commandHandler.Execute(gameCommand));

        Assert.NotNull(chatMessageEvent);
        Assert.Equal(DEFAULT_CHATTER_DISPLAY_NAME,
            chatMessageEvent.Arguments.ChatterDisplayName);
        Assert.Equal("You hired 2 Sales employees.",
            chatMessageEvent.Arguments.Message);

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal(2, company.Employees.Count);
        Assert.Equal(2, company.Points);
    }

    private HireCommandHandler GetHireCommandHandler(int points = 0)
    {
        Dictionary<string, Company> companies = new();

        companies.Add(DEFAULT_CHATTER_ID,
            new Company
            {
                ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
                CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
                Points = points
            });

        return new HireCommandHandler(_gameSettings, companies);
    }
}