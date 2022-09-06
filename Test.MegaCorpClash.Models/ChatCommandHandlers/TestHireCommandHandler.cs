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
        _gameSettings.EmployeeHiringDetails.Add(new EmployeeHiringDetails
        {
            Type = EmployeeType.Sales, 
            CostToHire = 10
        });

        _gameSettings.EmployeeHiringDetails.Add(new EmployeeHiringDetails
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

    [Fact]
    public void Test_NoParameters()
    {
        HireCommandHandler commandHandler = 
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire");

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
    public void Test_OneNumericParameter()
    {
        HireCommandHandler commandHandler = 
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire 1");

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
    public void Test_OneTextParameter()
    {
        HireCommandHandler commandHandler = 
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire asd");

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
    public void Test_TwoTextParameters()
    {
        HireCommandHandler commandHandler = 
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire asd qwe");

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
    public void Test_ThreeParameters()
    {
        HireCommandHandler commandHandler = 
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire 1 2 3");

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
    public void Test_InvalidJobType()
    {
        HireCommandHandler commandHandler = 
            GetHireCommandHandler();

        var gameCommand = GetGameCommand("!hire CEO 1");

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
        Assert.Equal("Congratulations! You hired 1 Sales employee.",
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
        Assert.Equal("Congratulations! You hired 2 Sales employees.",
            chatMessageEvent.Arguments.Message);

        var company = companies[DEFAULT_CHATTER_ID];

        Assert.Equal(2, company.Employees.Count);
        Assert.Equal(2, company.Points);
    }

    private HireCommandHandler GetHireCommandHandler()
    {
        Dictionary<string, Company> companies = new();

        companies.Add(DEFAULT_CHATTER_ID, new Company
        {
            ChatterName = DEFAULT_CHATTER_DISPLAY_NAME,
            CompanyName = DEFAULT_CHATTER_DISPLAY_NAME
        });

        return new HireCommandHandler(_gameSettings, companies);
    }
}