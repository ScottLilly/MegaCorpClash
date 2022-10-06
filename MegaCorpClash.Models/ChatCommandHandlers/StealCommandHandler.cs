using CSharpExtender.ExtensionMethods;
using CSharpExtender.Services;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers
{
    public class StealCommandHandler : BaseCommandHandler
    {
        public StealCommandHandler(GameSettings gameSettings,
            Dictionary<string, Company> companies)
            : base("steal", gameSettings, companies)
        {
        }

        public override void Execute(GameCommandArgs gameCommand)
        {
            var chatter = ChatterDetails(gameCommand);

            if (chatter.Company == null)
            {
                PublishMessage(chatter.ChatterName,
                    Literals.YouDoNotHaveACompany);
                return;
            }

            // Check if player has a Spy
            if (chatter.Company.Employees.None(e => e.Type == EmployeeType.Spy))
            {
                PublishMessage(chatter.ChatterName,
                    "You must have at least one Spy to steal");
                return;
            }

            var parsedArguments =
                _argumentParser.Parse(gameCommand.Argument);

            int numberOfAttackingSpies = 1;

            if (parsedArguments.IntegerArguments.Count == 1)
            {
                if (parsedArguments.IntegerArguments.First() > 1)
                {
                    numberOfAttackingSpies = 
                        Math.Min(
                            parsedArguments.IntegerArguments.First(), 
                            chatter.Company.Employees.First(e => e.Type == EmployeeType.Spy).Quantity);
                }
                else
                {
                    PublishMessage(chatter.ChatterName,
                        "Number of attacking spies must be greater than 0");
                    return;
                }
            }
            else if (parsedArguments.StringArguments.Any(s => s.Matches("max")))
            {
                numberOfAttackingSpies =
                    chatter.Company.Employees
                        .First(e => e.Type == EmployeeType.Spy).Quantity;
            }

            int successCount = 0;
            int failureCount = 0;
            long totalPointsStolen = 0;

            var broadcasterCompany = GetBroadcasterCompany;

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

                // Determine success of attack
                int securityEmployeeCount =
                    broadcasterCompany.Employees
                        .Where(e => e.Type == EmployeeType.Security)
                        .Sum(e => e.Quantity)
                    + 1;

                int broadcasterDefenseBonus =
                    Math.Max(25, Convert.ToInt32(Math.Log10(securityEmployeeCount) * 10));

                int rand = RngCreator.GetNumberBetween(1, 100);

                if (rand > 50 + broadcasterDefenseBonus)
                {
                    // Success
                    int stolen = (int)broadcasterCompany.Points / 100;

                    chatter.Company.Points += stolen;
                    broadcasterCompany.Points -= stolen;

                    successCount++;
                    totalPointsStolen += stolen;
                }
                else
                {
                    // Failure
                    // "Consume" broadcaster security person
                    var securityEmployeeQuantity =
                        broadcasterCompany.Employees
                            .FirstOrDefault(e => e.Type == EmployeeType.Security);

                    if (securityEmployeeQuantity == null)
                    {
                    }
                    else if (securityEmployeeQuantity.Quantity == 1)
                    {
                        broadcasterCompany.Employees.Remove(securityEmployeeQuantity);
                    }
                    else
                    {
                        securityEmployeeQuantity.Quantity--;
                    }

                    failureCount++;
                }
            }

            if (numberOfAttackingSpies == 1)
            {
                PublishMessage(chatter.ChatterName,
                    successCount == 1
                        ? $"Your spy stole {totalPointsStolen:N0} {GameSettings.PointsName}"
                        : $"Your spy was caught and you got nothing");
            }
            else
            {
                PublishMessage(chatter.ChatterName,
                    $"You had {successCount}/{numberOfAttackingSpies} successful attacks and stole {totalPointsStolen:N0} {GameSettings.PointsName}");
            }

            NotifyPlayerDataUpdated();
        }
    }
}