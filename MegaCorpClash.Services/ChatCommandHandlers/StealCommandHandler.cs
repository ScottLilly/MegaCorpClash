using CSharpExtender.ExtensionMethods;
using CSharpExtender.Services;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class StealCommandHandler : BaseCommandHandler
{
    public StealCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("steal", gameSettings, companies)
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
        long totalPointsStolen = 0;

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            // "Consume" spy
            var spyEmployeeQuantity =
                chatter.Company.Employees
                    .First(e => e.Type == EmployeeType.Spy);

            if (spyEmployeeQuantity.Quantity == 1)
            {
                chatter.Company.Employees.Remove(spyEmployeeQuantity);
            }
            else
            {
                spyEmployeeQuantity.Quantity--;
            }

            var attackSuccessful = IsAttackSuccessful(EmployeeType.Security);

            if (attackSuccessful)
            {
                // Success
                int stolen = GetBroadcasterCompany.Points / RngCreator.GetNumberBetween(100, 500);

                if (GetBroadcasterCompany.Points < 1000)
                {
                    stolen = GetBroadcasterCompany.Points;
                }

                chatter.Company.Points += stolen;
                GetBroadcasterCompany.Points -= stolen;

                successCount++;
                totalPointsStolen += stolen;
            }
            else
            {
                // Failure, consumes broadcaster security person
                var securityEmployeeQuantity =
                    GetBroadcasterCompany.Employees
                        .FirstOrDefault(e => e.Type == EmployeeType.Security);

                if (securityEmployeeQuantity == null)
                {
                }
                else if (securityEmployeeQuantity.Quantity == 1)
                {
                    GetBroadcasterCompany.Employees.Remove(securityEmployeeQuantity);
                }
                else
                {
                    securityEmployeeQuantity.Quantity--;
                }
            }
        }

        if (numberOfAttackingSpies == 1)
        {
            PublishMessage(successCount == 1
                    ? $"Your spy stole {totalPointsStolen:N0} {GameSettings.PointsName} and now have {chatter.Company.Points:N0} {GameSettings.PointsName}"
                    : "Your spy was caught and you got nothing");
        }
        else
        {
            PublishMessage($"You had {successCount:N0}/{numberOfAttackingSpies:N0} successful attacks and stole {totalPointsStolen:N0} {GameSettings.PointsName} and now have {chatter.Company.Points:N0} {GameSettings.PointsName}");
        }

        if (GetBroadcasterCompany.Points == 0)
        {
            NotifyBankruptedStreamer();
        }

        NotifyCompanyDataUpdated();
    }

    private int GetNumberOfAttackingSpies(GameCommandArgs gameCommand, Company company)
    {
        int numberOfAttackingSpies = 1;

        var parsedArguments =
            _argumentParser.Parse(gameCommand.Argument);

        if (parsedArguments.IntegerArguments.Count == 1)
        {
            if (parsedArguments.IntegerArguments.First() > 1)
            {
                numberOfAttackingSpies =
                    Math.Min(
                parsedArguments.IntegerArguments.First(),
                        company.Employees.First(e => e.Type == EmployeeType.Spy).Quantity);
            }
            else
            {
                numberOfAttackingSpies = 0;
            }
        }
        else if (parsedArguments.StringArguments.Any(s => s.Matches("max")))
        {
            numberOfAttackingSpies =
                company.Employees
                    .First(e => e.Type == EmployeeType.Spy).Quantity;
        }

        return numberOfAttackingSpies;
    }
}