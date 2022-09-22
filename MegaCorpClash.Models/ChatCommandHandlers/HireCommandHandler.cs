using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class HireCommandHandler : BaseCommandHandler
{
    private readonly ArgumentParser _argumentParser = new();

    public HireCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("hire", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        if(chatter.Company == null)
        {
            PublishMessage(chatter.ChatterName, 
                Literals.YouDoNotHaveACompany);
            return;
        }

        // Check that we have valid parameters
        var parsedArguments = 
            _argumentParser.Parse(gameCommand.Argument);

        if(parsedArguments.IntegerArguments.Count > 1 ||
           parsedArguments.EnumArgumentsOfType<EmployeeType>().Count() != 1)
        {
            PublishMessage(chatter.ChatterName, Literals.Hire_InvalidParameters);
            return;
        }

        // Determine quantity of employees to hire
        // Default to 1, if no quantity passed in command
        int qtyToHire = 
            parsedArguments.IntegerArguments.Any() 
                ? parsedArguments.IntegerArguments.First() 
                : 1;

        var empType = 
            parsedArguments.EnumArgumentsOfType<EmployeeType>().First();

        // Determine cost to hire and check if has enough points
        int costToHireOne = 
            GameSettings.EmployeeHiringDetails
                .First(ehd => ehd.Type == empType).CostToHire;

        // Handle if chatter entered "max" for the qty
        if (parsedArguments.IntegerArguments.None() &&
            parsedArguments.StringArguments.Any(sa => sa.Matches("max")))
        {
            qtyToHire = 
                (int)Math.Round(chatter.Company.Points / (decimal)costToHireOne, 
                    MidpointRounding.ToZero);
        }

        if (qtyToHire < 1)
        {
            PublishMessage(chatter.ChatterName,
                Literals.Hire_QuantityMustBeGreaterThanZero);
            return;
        }

        int? costToHire = 
            costToHireOne * qtyToHire;

        if (costToHire > chatter.Company.Points)
        {
            PublishMessage(chatter.ChatterName,
                $"It costs {costToHire} {GameSettings.PointsName} to hire {qtyToHire} {empType} employees. You only have {chatter.Company.Points} {GameSettings.PointsName}");
            return;
        }

        // Success! Hire the employee(s)
        chatter.Company.Points -= (int)costToHire;

        var empQtyObject = 
            chatter.Company.Employees.FirstOrDefault(e => e.Type == empType);

        if (empQtyObject == null)
        {
            chatter.Company.Employees
                .Add(new EmployeeQuantity
                {
                    Type = empType,
                    Quantity = qtyToHire
                });
        }
        else
        {
            empQtyObject.Quantity += qtyToHire;
        }

        NotifyPlayerDataUpdated();

        string message =
            $"You hired {qtyToHire} {empType} employee" +
            (qtyToHire == 1 ? "" : "s") + 
            $" and have {chatter.Company.Points:N0} {GameSettings.PointsName} remaining.";

        PublishMessage(chatter.ChatterName, message);
    }
}