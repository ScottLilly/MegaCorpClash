using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class RecruitCommandHandler : BaseCommandHandler
{
    private GameSettings.AttackDetail _attackDetail;

    public RecruitCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("recruit", gameSettings, companies)
    {
        BroadcasterCanRun = false;

        _attackDetail =
            GameSettings.AttackDetails?
            .FirstOrDefault(ad => ad.AttackType.Matches(CommandName))
            ?? new GameSettings.AttackDetail { Min = 1, Max = 2 };
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

        var chatter = ChatterDetails(gameCommandArgs);

        if (chatter.Company == null)
        {
            PublishMessage(Literals.YouDoNotHaveACompany);
            return;
        }

        // Check if player's company has a Spy
        if (chatter.Company.Employees.None(e => e.Type == EmployeeType.Spy))
        {
            PublishMessage("You must have at least one spy to recruit employees");
            return;
        }

        int numberOfAttackingSpies =
            GetNumberOfAttackingSpies(gameCommandArgs, chatter.Company);

        if (numberOfAttackingSpies < 1)
        {
            PublishMessage("Number of attacking spies must be greater than 0");
            return;
        }

        int successCount = 0;
        List<EmployeeQuantity> employeesWhoStrike = new List<EmployeeQuantity>();

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            // "Consume" spy during attack
            chatter.Company.RemoveEmployeeOfType(EmployeeType.Spy);

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
                        GetBroadcasterCompany.RemoveEmployeeOfType(emp.Type, employeesLeaving);

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
                GetBroadcasterCompany.RemoveEmployeeOfType(EmployeeType.HR);
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

        if(successCount > 0)
        {
            NotifyCompanyDataUpdated();
        }
    }
}