using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;
using System.Reflection.Metadata;

namespace Test.MegaCorpClash.Services.ChatCommandHandlers;

public class TestHireCommandHandler : BaseCommandHandlerTest
{
    [Theory]
    [MemberData(nameof(InvalidParameterTestValues))]
    public void Test_InvalidParameters(string parameter)
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "hire", parameter);

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Hire_InvalidParameters);
    }

    [Fact]
    public void Test_NoCompany()
    {
        // TODO: FIX!
        //// Setup
        //var repo = GetTestInMemoryRepository();
        //var args = GetGameCommandArgs(new Company(), "hire", "");

        //var commandHandler =
        //    new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        //commandHandler.Execute();

        //commandHandler.ChatMessages.First()
        //    .ShouldBe(Literals.YouDoNotHaveACompany);
    }

    [Fact]
    public void Test_OneValidOneInvalid()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "hire", "Sales gibberish");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You hired 1 Sales employee and have 999,950 CorpoBux remaining.");
    }

    [Fact]
    public void Test_OneValidTextParameter()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "hire", "sales");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You hired 1 Sales employee and have 999,950 CorpoBux remaining.");
    }

    [Fact]
    public void Test_ZeroQuantity()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "hire", "sales 0");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Hire_QuantityMustBeGreaterThanZero);
    }

    [Fact]
    public void Test_NegativeQuantity()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var args = GetGameCommandArgs(repo.GetBroadcasterCompany(), "hire", "sales -1");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe(Literals.Hire_QuantityMustBeGreaterThanZero);
    }

    [Fact]
    public void Test_ValidParametersInsufficientMoney()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101"); // Should have 1000 CorpoBux
        var args = GetGameCommandArgs(company, "hire", "sales 21");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("It costs 1,050 CorpoBux to hire 21 Sales employees. You only have 1,000 CorpoBux");
    }

    [Fact]
    public void Test_ValidParametersInsufficientMoneyToHireMax()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        repo.SubtractPoints("101", 960);
        var company = repo.GetCompany("101"); // Should have 40 CorpoBux
        var args = GetGameCommandArgs(company, "hire", "sales max");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("It costs 50 CorpoBux to hire a Sales employee. You only have 40 CorpoBux");
    }

    [Fact]
    public void Test_ValidParametersMaxHireSufficientMoney()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        repo.SubtractPoints("101", 950);
        var company = repo.GetCompany("101"); // Should have 50 CorpoBux
        var args = GetGameCommandArgs(company, "hire", "sales max");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First()
            .ShouldBe("You hired 1 Sales employee and have 0 CorpoBux remaining.");
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_OneEmployee()
    {
        //Dictionary<string, Company> companies = new();

        //companies.Add(
        //    DEFAULT_CHATTER_ID,
        //    new Company
        //    {
        //        DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
        //        CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
        //        Points = 11
        //    });

        //var commandHandler =
        //    new HireCommandHandler(_gameSettings, companies);

        //var gameCommand = GetGameCommandArgs("!hire Sales 1");

        //commandHandler.Execute(gameCommand);

        //Assert.Equal("You hired 1 Sales employee and have 1 CorpoBux remaining.",
        //    commandHandler.ChatMessages.First());

        //var company = companies[DEFAULT_CHATTER_ID];

        //Assert.Single(company.Employees);
        //Assert.Equal(1, company.Points);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_OneEmployee_WithHRDiscount()
    {
        //Dictionary<string, Company> companies = new();

        //companies.Add(
        //    DEFAULT_CHATTER_ID,
        //    new Company
        //    {
        //        DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
        //        CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
        //        Points = 11,
        //        Employees = new List<EmployeeQuantity>
        //        {
        //            new()
        //            {
        //                Type = EmployeeType.HR,
        //                Quantity = 10
        //            }
        //        }
        //    });

        //var commandHandler =
        //    new HireCommandHandler(_gameSettings, companies);

        //var gameCommand = GetGameCommandArgs("!hire Sales 1");

        //commandHandler.Execute(gameCommand);

        //Assert.Equal("You hired 1 Sales employee and have 2 CorpoBux remaining.",
        //    commandHandler.ChatMessages.First());

        //var company = companies[DEFAULT_CHATTER_ID];

        //Assert.Equal(2, company.Employees.Count);
        //Assert.Equal(2, company.Points);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_TwoEmployees()
    {
        //Dictionary<string, Company> companies = new();

        //companies.Add(
        //    DEFAULT_CHATTER_ID,
        //    new Company
        //    {
        //        DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
        //        CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
        //        Points = 22
        //    });

        //var commandHandler =
        //    new HireCommandHandler(_gameSettings, companies);

        //var gameCommand = GetGameCommandArgs("!hire 2 sales");

        //commandHandler.Execute(gameCommand);

        //Assert.Equal("You hired 2 Sales employees and have 2 CorpoBux remaining.",
        //    commandHandler.ChatMessages.First());

        //var company = companies[DEFAULT_CHATTER_ID];

        //Assert.Equal(2, company.Employees.First(e => e.Type == EmployeeType.Sales).Quantity);
        //Assert.Equal(2, company.Points);
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

    //private HireCommandHandler GetHireCommandHandler(int points = 0)
    //{
    //    Dictionary<string, Company> companies = new();

    //    companies.Add(DEFAULT_CHATTER_ID,
    //        new Company
    //        {
    //            DisplayName = DEFAULT_CHATTER_DISPLAY_NAME,
    //            CompanyName = DEFAULT_CHATTER_DISPLAY_NAME,
    //            Points = points
    //        });

    //    return new HireCommandHandler(_gameSettings, companies);
    //}
}