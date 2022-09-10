﻿namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HireCommandHandler : BaseCommandHandler
{
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

        if(gameCommand.ArgumentsAsList.Count is < 1 or > 2)
        {
            PublishMessage(chatter.ChatterName, Literals.Hire_InvalidParameters);
            return;
        }

        var hiringDetails = GetHiringDetails(gameCommand.ArgumentsAsList);

        if(hiringDetails.Job == null)
        {
            PublishMessage(chatter.ChatterName, Literals.Hire_InvalidParameters);
            return;
        }

        if (hiringDetails.Qty < 1)
        {
            PublishMessage(chatter.ChatterName,
                Literals.Hire_QuantityMustBeGreaterThanZero);
            return;
        }

        var empHiringDetails = 
            GameSettings.EmployeeHiringDetails
                .First(ehd => ehd.Type == hiringDetails.Job);

        int? hiringCost = empHiringDetails.CostToHire * hiringDetails.Qty;

        if (hiringCost > chatter.Company.Points)
        {
            PublishMessage(chatter.ChatterName,
                $"It costs {hiringCost} {GameSettings.PointsName} to hire {hiringDetails.Qty} {hiringDetails.Job} employees. You only have {chatter.Company.Points} {GameSettings.PointsName}");
            return;
        }

        chatter.Company.Points -= (int)hiringCost;
        for(int i = 0; i < hiringDetails.Qty; i++)
        {
            chatter.Company.Employees
                .Add(new Employee 
                { 
                    Type = empHiringDetails.Type, 
                    SkillLevel = 1
                });
        }

        NotifyPlayerDataUpdated();

        string message =
            $"Congratulations! You hired {hiringDetails.Qty} {hiringDetails.Job} employee" +
            (hiringDetails.Qty == 1 ? "." : "s.");

        PublishMessage(chatter.ChatterName, message);
    }

    private static (EmployeeType? Job, int? Qty) GetHiringDetails(List<string> arguments)
    {
        int? qty = 1;
        EmployeeType? employeeType = null;

        var employeeTypeArgument = arguments[0];
        if (int.TryParse(arguments[0], out int qty0))
        {
            qty = qty0;
            if (arguments.Count < 2) return (employeeType, qty);
            employeeTypeArgument = arguments[1];
        }
        else if (arguments.Count > 1 && int.TryParse(arguments[1], out var qty1))
        {
            qty = qty1;
        }
        if (Enum.TryParse(employeeTypeArgument, true, out EmployeeType emp0))
        {
            employeeType = emp0;
        }
        return (employeeType, qty);
    }
}