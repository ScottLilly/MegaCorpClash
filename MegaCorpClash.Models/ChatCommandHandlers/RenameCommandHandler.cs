using CSharpExtender.ExtensionMethods;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class RenameCommandHandler : BaseCommandHandler
{
    public RenameCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("rename", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        if (chatter.Company == null)
        {
            PublishMessage(chatter.Name, Literals.YouDoNotHaveACompany);
            return;
        }

        string newCompanyName = gameCommand.Argument;

        if (string.IsNullOrWhiteSpace(newCompanyName))
        {
            PublishMessage(chatter.Name,
                Literals.Rename_YouMustProvideANewName);
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(newCompanyName)))
        {
            PublishMessage(chatter.Name, 
                $"There is already a company named {newCompanyName}");
            return;
        }

        chatter.Company.CompanyName = newCompanyName;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
            $"Your company is now named {chatter.Company.CompanyName}");
    }
}