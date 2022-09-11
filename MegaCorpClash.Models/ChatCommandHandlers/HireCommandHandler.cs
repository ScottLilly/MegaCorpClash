using MegaCorpClash.Core;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HireCommandHandler : BaseCommandHandler
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

        if (qtyToHire < 1)
        {
            PublishMessage(chatter.ChatterName,
                Literals.Hire_QuantityMustBeGreaterThanZero);
            return;
        }

        var empType = 
            parsedArguments.EnumArgumentsOfType<EmployeeType>().First();

        // Determine cost to hire and check if has enough points
        int? costToHire = 
            GameSettings.EmployeeHiringDetails
                .First(ehd => ehd.Type == empType).CostToHire * qtyToHire;

        if (costToHire > chatter.Company.Points)
        {
            PublishMessage(chatter.ChatterName,
                $"It costs {costToHire} {GameSettings.PointsName} to hire {qtyToHire} {empType} employees. You only have {chatter.Company.Points} {GameSettings.PointsName}");
            return;
        }

        // Success! Hire the employee(s)
        chatter.Company.Points -= (int)costToHire;
        for(int i = 0; i < qtyToHire; i++)
        {
            chatter.Company.Employees
                .Add(new Employee 
                { 
                    Type = empType, 
                    SkillLevel = 1
                });
        }

        NotifyPlayerDataUpdated();

        string message =
            $"You hired {qtyToHire} {empType} employee" +
            (qtyToHire == 1 ? "." : "s.");

        PublishMessage(chatter.ChatterName, message);
    }
}