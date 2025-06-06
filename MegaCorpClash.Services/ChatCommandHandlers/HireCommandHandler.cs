﻿using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class HireCommandHandler : BaseCommandHandler
{
    public HireCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("hire", gameSettings, companyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        var chatter = ChatterDetails();

        if (chatter.Company == null)
        {
            PublishMessage(Literals.YouDoNotHaveACompany);
            return;
        }

        // Check that we have valid parameters
        var parsedArguments =
            _argumentParser.Parse(GameCommandArgs.Argument);

        if (parsedArguments.IntegerArguments.Count > 1 ||
           parsedArguments.EnumArgumentsOfType<EmployeeType>().Count() != 1)
        {
            PublishMessage(Literals.Hire_InvalidParameters);
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

        // Apply HR discount
        int hrEmployeeCount =
            chatter.Company.Employees
                .Where(e => e.Type == EmployeeType.HR)
                .Sum(e => e.Quantity)
            + 1;

        int discount = Convert.ToInt32(Math.Log10(hrEmployeeCount) * 10);

        costToHireOne =
            Convert.ToInt32(Convert.ToDecimal(costToHireOne) *
                            Convert.ToDecimal(Math.Max(100 - discount, GameSettings.LowestHrDiscount) / 100M));

        // Handle if chatter entered "max" for the qty
        if (parsedArguments.IntegerArguments.None() &&
            parsedArguments.StringArguments.Any(sa => sa.Matches("max")))
        {
            qtyToHire =
                (int)Math.Round(chatter.Company.Points / (decimal)costToHireOne,
                    MidpointRounding.ToZero);

            if (qtyToHire < 1)
            {
                PublishMessage($"It costs {costToHireOne:N0} {GameSettings.PointsName} to hire a {empType} employee. You only have {chatter.Company.Points:N0} {GameSettings.PointsName}");
                return;
            }
        }

        if (qtyToHire < 1)
        {
            PublishMessage(Literals.Hire_QuantityMustBeGreaterThanZero);
            return;
        }

        int? costToHire = costToHireOne * qtyToHire;

        if (costToHire > chatter.Company.Points)
        {
            PublishMessage($"It costs {costToHire:N0} {GameSettings.PointsName} to hire {qtyToHire:N0} {empType} employees. You only have {chatter.Company.Points:N0} {GameSettings.PointsName}");
            return;
        }

        // Success! Hire the employee(s)
        CompanyRepository.HireEmployees(chatter.ChatterId, 
            empType, qtyToHire, (int)costToHire);

        var updatedCompany = CompanyRepository.GetCompany(chatter.ChatterId);

        string message =
            $"You hired {qtyToHire:N0} {empType} employee" +
            (qtyToHire == 1 ? "" : "s") +
            $" and have {updatedCompany.Points:N0} {GameSettings.PointsName} remaining.";

        PublishMessage(message);
    }
}