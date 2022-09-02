using CSharpExtender.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class IncorporateCommandHandler : BaseCommandHandler
{
    public IncorporateCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> players)
        : base("incorporate", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        var chatter = ChatterDetails(chatCommand);
        string? companyName = chatCommand.ArgumentsAsString;

        if (string.IsNullOrWhiteSpace(companyName))
        {
            PublishMessage(chatter.Name, 
                Literals.Incorporate_NameRequired);
            return;
        }

        if (chatter.Company != null)
        {
            PublishMessage(chatter.Name,
                $"You already have a company named {chatter.Company.CompanyName}");
            return;
        }

        if (Players.Values.Any(p => p.CompanyName.Matches(companyName)))
        {
            PublishMessage(chatter.Name,
                $"There is already a company named {companyName}");
            return;
        }

        chatter.Company = 
            new Company
            {
                ChatterId = chatter.Id,
                ChatterName = chatter.Name,
                CompanyName = companyName,
                CreatedOn = DateTime.UtcNow,
                Employees = new List<Employee>
                {
                    new() { Type = EmployeeType.Production, SkillLevel = 1 },
                    new() { Type = EmployeeType.Sales, SkillLevel = 1 },
                }

            };

        Players[chatter.Id] = chatter.Company;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
            $"You are now the proud CEO of {companyName}");
    }
}