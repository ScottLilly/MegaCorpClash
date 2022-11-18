using CSharpExtender.ExtensionMethods;
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

        // Check if player's company has a Spy
        if (chatter.Company.Employees.None(e => e.Type == EmployeeType.Spy))
        {
            PublishMessage("You must have at least one spy to initiate a strike");
            return;
        }

        int numberOfAttackingSpies =
            GetNumberOfAttackingSpies(GameCommandArgs, chatter.Company);

        if (numberOfAttackingSpies < 1)
        {
            PublishMessage("Number of attacking spies must be greater than 0");
            return;
        }

        int successCount = 0;
        int employeesWhoLeft = 0;

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

                        employeesWhoLeft += employeesLeaving;
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