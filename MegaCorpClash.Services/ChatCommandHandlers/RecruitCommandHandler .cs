﻿using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class RecruitCommandHandler : BaseCommandHandler
{
    private GameSettings.AttackDetail _attackDetail;

    public RecruitCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("recruit", gameSettings, companyRepository, gameCommandArgs)
    {
        BroadcasterCanRun = false;

        _attackDetail =
            GameSettings.AttackDetails?
            .FirstOrDefault(ad => ad.AttackType.Matches(CommandName))
            ?? new GameSettings.AttackDetail { Min = 1, Max = 2 };
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

        int numberOfAttackingSpies =
            GetNumberOfAttackingSpies(GameCommandArgs, chatter.Company);

        if (numberOfAttackingSpies < 1)
        {
            PublishMessage("Number of attacking spies must be greater than 0");
            return;
        }

        // Check if player's company has enough spies
        int availableSpies =
            chatter.Company.Employees.First(e => e.Type == EmployeeType.Spy)?.Quantity ?? 0;

        if (availableSpies < numberOfAttackingSpies)
        {
            if (availableSpies == 0)
            {
                PublishMessage("You don't have any spies");
            }
            else if (availableSpies == 1)
            {
                PublishMessage("You only have 1 spy");
            }
            else
            {
                PublishMessage($"You only have {availableSpies} spies");
            }
            return;
        }
        int successCount = 0;
        List<EmployeeQuantity> employeesWhoStrike = new List<EmployeeQuantity>();

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            // "Consume" spy during attack
            CompanyRepository.RemoveEmployeeOfType(chatter.ChatterId, EmployeeType.Spy);

            var attackSuccessful = IsAttackSuccessful(EmployeeType.HR);

            if (attackSuccessful)
            {
                var employeesByCost = 
                    GameSettings.EmployeeHiringDetails.OrderBy(ehd => ehd.CostToHire);

                foreach(var emp in employeesByCost)
                {
                    var broadcasterEmpQty = 
                        GetBroadcasterCompany.Employees.FirstOrDefault(e => e.Type == emp.Type);

                    int employeesLeaving = 
                        Random.Shared.Next(_attackDetail.Min, _attackDetail.Max + 1);

                    if(broadcasterEmpQty != null &&
                        broadcasterEmpQty.Quantity > employeesLeaving)
                    {
                        CompanyRepository.RemoveEmployeeOfType(
                            GetBroadcasterCompany.UserId, emp.Type, employeesLeaving);
                        CompanyRepository.HireEmployees(
                            chatter.ChatterId, emp.Type, employeesLeaving, 0);

                        var employeeTypeWhoStruck = 
                            employeesWhoStrike.FirstOrDefault(e => e.Type.Equals(emp.Type));
                        
                        if(employeeTypeWhoStruck == null)
                        {
                            employeesWhoStrike.Add(
                                new EmployeeQuantity 
                                {
                                    Type = emp.Type, 
                                    Quantity = employeesLeaving 
                                });
                        }
                        else
                        {
                            employeeTypeWhoStruck.Quantity += employeesLeaving;
                        }

                        successCount++;
                        break;
                    }
                }

            }
            else
            {
                // Failure, consumes broadcaster HR employee
                CompanyRepository.RemoveEmployeeOfType(
                    GetBroadcasterCompany.UserId, EmployeeType.HR);
            }
        }

        string recruits = 
            string.Join(',', 
            employeesWhoStrike.Select(e => $"{e.Quantity:N0} {e.Type}"));

        if (numberOfAttackingSpies == 1)
        {
            PublishMessage(successCount == 1
                    ? $"Your spy recruited {recruits} employees from {GetBroadcasterCompany.CompanyName}"
                    : $"Your spy was caught and you didn't recruit anyone from {GetBroadcasterCompany.CompanyName}");
        }
        else
        {
            PublishMessage($"You had {successCount:N0}/{numberOfAttackingSpies:N0} successful attacks and recruited {recruits} employees from {GetBroadcasterCompany.CompanyName}");
        }
    }
}