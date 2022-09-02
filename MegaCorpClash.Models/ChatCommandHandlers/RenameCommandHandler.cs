using CSharpExtender.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class RenameCommandHandler : BaseCommandHandler
{
    public RenameCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> players)
        : base("rename", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        var chatter = ChatterDetails(chatCommand);

        if (chatter.Company == null)
        {
            PublishMessage(chatter.Name, Literals.YouDoNotHaveACompany);
            return;
        }

        string newCompanyName = chatCommand.ArgumentsAsString;

        if (string.IsNullOrWhiteSpace(newCompanyName))
        {
            PublishMessage(chatter.Name,
                Literals.Rename_YouMustProvideANewName);
            return;
        }

        if (Players.Values.Any(p => p.CompanyName.Matches(newCompanyName)))
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