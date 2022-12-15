using MegaCorpClash.Models;
using MegaCorpClash.Services.ChatCommandHandlers;
using Shouldly;

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
        // Setup
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101"); // Should have 1000 CorpoBux
        var args = GetGameCommandArgs(company, "hire", "sales 1");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First().
            ShouldBe("You hired 1 Sales employee and have 950 CorpoBux remaining.");

        var updatedCompany = repo.GetCompany("101");
        updatedCompany.Employees
            .First(e => e.Type == EmployeeType.Sales).Quantity
            .ShouldBe(11);
        updatedCompany.Points.ShouldBe(950);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_OneEmployee_WithHRDiscount()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        repo.HireEmployees("101", EmployeeType.HR, 10, 0);
        var company = repo.GetCompany("101"); // Should have 1000 CorpoBux
        var args = GetGameCommandArgs(company, "hire", "sales 1");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First().
            ShouldBe("You hired 1 Sales employee and have 955 CorpoBux remaining.");

        var updatedCompany = repo.GetCompany("101");
        updatedCompany.Employees
            .First(e => e.Type == EmployeeType.Sales).Quantity
            .ShouldBe(11);
        updatedCompany.Points.ShouldBe(955);
    }

    [Fact]
    public void Test_ValidParametersSufficientMoney_TwoEmployees()
    {
        // Setup
        var repo = GetTestInMemoryRepository();
        var company = repo.GetCompany("101"); // Should have 1000 CorpoBux
        var args = GetGameCommandArgs(company, "hire", "sales 2");

        var commandHandler =
            new HireCommandHandler(GetDefaultGameSettings(), repo, args);

        commandHandler.Execute();

        commandHandler.ChatMessages.First().
            ShouldBe("You hired 2 Sales employees and have 900 CorpoBux remaining.");

        var updatedCompany = repo.GetCompany("101");
        updatedCompany.Employees
            .First(e => e.Type == EmployeeType.Sales).Quantity
            .ShouldBe(12);
        updatedCompany.Points.ShouldBe(900);
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

}