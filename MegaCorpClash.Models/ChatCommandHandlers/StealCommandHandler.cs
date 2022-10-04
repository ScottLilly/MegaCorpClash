using CSharpExtender.ExtensionMethods;
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

            // Check player has a Spy
            if (chatter.Company.Employees.None(e => e.Type == EmployeeType.Spy))
            {
                PublishMessage(chatter.ChatterName,
                    "You must have at least one Spy to steal");
                return;
            }

        }
    }
}