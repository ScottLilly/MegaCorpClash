using CSharpExtender.ExtensionMethods;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class IncorporateCommandHandler : BaseCommandHandler
{
    public IncorporateCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("incorporate", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);
        string? companyName = gameCommand.Argument;

        if (string.IsNullOrWhiteSpace(companyName))
        {
            PublishMessage(chatter.ChatterName, 
                Literals.Incorporate_NameRequired);
            return;
        }

        if (chatter.Company != null)
        {
            PublishMessage(chatter.ChatterName,
                $"You already have a company named {chatter.Company.CompanyName}");
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(companyName)))
        {
            PublishMessage(chatter.ChatterName,
                $"There is already a company named {companyName}");
            return;
        }

        chatter.Company = 
            new Company
            {
                ChatterId = chatter.ChatterId,
                ChatterName = chatter.ChatterName,
                CompanyName = companyName,
                CreatedOn = DateTime.UtcNow,
                Employees = new List<Employee>
                {
                    new() { Type = EmployeeType.Production, SkillLevel = 1 },
                    new() { Type = EmployeeType.Sales, SkillLevel = 1 },
                }

            };

        Companies[chatter.ChatterId] = chatter.Company;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.ChatterName,
            $"You are now the proud CEO of {companyName}");
    }
}