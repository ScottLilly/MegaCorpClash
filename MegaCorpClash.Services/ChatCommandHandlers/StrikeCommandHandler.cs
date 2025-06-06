﻿using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class StrikeCommandHandler : BaseCommandHandler
{
    private GameSettings.AttackDetail _attackDetail;

    public StrikeCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("strike", gameSettings, companyRepository, gameCommandArgs)
    {
        BroadcasterCanRun = false;

        _attackDetail =
            GameSettings.AttackDetails?
            .FirstOrDefault(ad => ad.AttackType.Matches(CommandName))
            ?? new GameSettings.AttackDetail { Min = 2, Max = 5 };
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

        int availableSpies = chatter.Company.EmployeesOfType(EmployeeType.Spy);

        // Check if player's company has enough spies
        if (availableSpies < numberOfAttackingSpies)
        {
            SetMessageForInsufficientSpies(availableSpies);
            return;
        }

        var employeesByCost =
            GameSettings.EmployeeHiringDetails.OrderBy(ehd => ehd.CostToHire);

        List<EmployeeQuantity> broadcasterEmployees = GetBroadcasterCompany.Employees.ToList();
        int successCount = 0;
        int employeesWhoLeft = 0;
        int hrPeopleConsumed = 0;

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            var attackSuccessful = IsAttackSuccessful(EmployeeType.HR);

            if (attackSuccessful)
            {
                foreach (var emp in employeesByCost)
                {
                    var broadcasterEmpQty =
                        broadcasterEmployees.FirstOrDefault(e => e.Type == emp.Type);

                    int employeesLeaving =
                        Random.Shared.Next(_attackDetail.Min, _attackDetail.Max + 1);

                    if (broadcasterEmpQty != null &&
                        broadcasterEmpQty.Quantity >= employeesLeaving)
                    {
                        if (broadcasterEmpQty.Quantity == employeesLeaving)
                        {
                            broadcasterEmployees.Remove(broadcasterEmpQty);
                        }
                        else
                        {
                            broadcasterEmpQty.Quantity -= employeesLeaving;
                        }

                        employeesWhoLeft += employeesLeaving;
                        successCount++;
                        break;
                    }
                }
            }
            else
            {
                // Failure, consumes broadcaster HR employee
                hrPeopleConsumed++;
            }
        }

        ApplyAttackResults(numberOfAttackingSpies, broadcasterEmployees, hrPeopleConsumed);

        SetResultMessage(numberOfAttackingSpies, successCount, employeesWhoLeft);
    }

    private void ApplyAttackResults(int numberOfAttackingSpies, 
        List<EmployeeQuantity> broadcasterEmployees, int hrPeopleConsumed)
    {
        var chatter = ChatterDetails();

        CompanyRepository.RemoveEmployeeOfType(chatter.ChatterId, EmployeeType.Spy, numberOfAttackingSpies);

        var broadcasterCompany = GetBroadcasterCompany;
        broadcasterCompany.Employees.Clear();
        foreach (var empQty in broadcasterEmployees)
        {
            broadcasterCompany.Employees.Add(empQty);
        }
        CompanyRepository.UpdateCompany(broadcasterCompany);

        CompanyRepository.RemoveEmployeeOfType(
            GetBroadcasterCompany.UserId, EmployeeType.HR, hrPeopleConsumed);
    }

    private void SetResultMessage(int numberOfAttackingSpies, int successCount, 
        int employeesWhoLeft)
    {
        if (numberOfAttackingSpies == 1)
        {
            PublishMessage(successCount == 1
                ? $"Your spy caused {employeesWhoLeft:N0} employees to leave {GetBroadcasterCompany.CompanyName}"
                : $"Your spy was caught and nobody went on strike at {GetBroadcasterCompany.CompanyName}");
        }
        else
        {
            PublishMessage($"You had {successCount:N0}/{numberOfAttackingSpies:N0} successful attacks and caused {employeesWhoLeft:N0} employees to leave {GetBroadcasterCompany.CompanyName}");
        }
    }
}