using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class StrikeCommandHandler : BaseCommandHandler
{
    public StrikeCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("strike", gameSettings, companies)
    {
        BroadcasterCanRun = false;
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
            PublishMessage("You must have at least one spy to steal");
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

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            // "Consume" spy during attack
            chatter.Company.RemoveEmployeeOfType(EmployeeType.Spy);

            var attackSuccessful = IsAttackSuccessful(EmployeeType.HR);

            if (attackSuccessful)
            {
                successCount++;
            }
            else
            {
                // Failure, consumes broadcaster HR employee
                GetBroadcasterCompany.RemoveEmployeeOfType(EmployeeType.HR);
            }
        }

        if (numberOfAttackingSpies == 1)
        {
            PublishMessage(successCount == 1
                    ? $"Your spy caused {successCount:N0} employees to leave {GetBroadcasterCompany.CompanyName}"
                    : $"Your spy was caught and nobody went on strike at {GetBroadcasterCompany.CompanyName}");
        }
        else
        {
            PublishMessage($"You had {successCount:N0}/{numberOfAttackingSpies:N0} successful attacks and caused {successCount:N0} employees to leave {GetBroadcasterCompany.CompanyName}");
        }

        if(successCount > 0)
        {
            //NotifyCompanyDataUpdated();
        }
    }
}