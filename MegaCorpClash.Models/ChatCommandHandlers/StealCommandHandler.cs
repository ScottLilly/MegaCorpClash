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

            var broadcasterCompany = GetBroadcasterCompany;

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

                PublishMessage(chatter.ChatterName,
                    $"You stole {stolen} {GameSettings.PointsName} from {broadcasterCompany.CompanyName}");
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
                else if(securityEmployeeQuantity.Quantity == 1)
                {
                    broadcasterCompany.Employees.Remove(securityEmployeeQuantity);
                }
                else
                {
                    securityEmployeeQuantity.Quantity--;
                }

                PublishMessage(chatter.ChatterName,
                    $"You failed to steal from {broadcasterCompany.CompanyName}");
            }

            NotifyPlayerDataUpdated();
        }
    }
}