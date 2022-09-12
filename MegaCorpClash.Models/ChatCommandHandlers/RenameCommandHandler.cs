using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public sealed class RenameCommandHandler : BaseCommandHandler
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
            PublishMessage(chatter.ChatterName, Literals.YouDoNotHaveACompany);
            return;
        }

        string newCompanyName = gameCommand.Argument;

        if (string.IsNullOrWhiteSpace(newCompanyName))
        {
            PublishMessage(chatter.ChatterName,
                Literals.Rename_YouMustProvideANewName);
            return;
        }

        if (!newCompanyName.IsSafeText())
        {
            PublishMessage(chatter.ChatterName,
                Literals.CompanyName_NotSafeText);
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(newCompanyName)))
        {
            PublishMessage(chatter.ChatterName, 
                $"There is already a company named {newCompanyName}");
            return;
        }

        chatter.Company.CompanyName = newCompanyName;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.ChatterName,
            $"Your company is now named {chatter.Company.CompanyName}");
    }
}