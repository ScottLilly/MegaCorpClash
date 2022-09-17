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
            PublishMessage(chatter.ChatterName, 
                Literals.YouDoNotHaveACompany);
            return;
        }

        if (gameCommand.DoesNotHaveArguments)
        {
            PublishMessage(chatter.ChatterName,
                Literals.Rename_YouMustProvideANewName);
            return;
        }

        if (gameCommand.Argument.IsNotSafeText())
        {
            PublishMessage(chatter.ChatterName,
                Literals.Incorporate_NotSafeText);
            return;
        }

        if (gameCommand.Argument.Length >
            GameSettings.MaxCompanyNameLength)
        {
            PublishMessage(chatter.ChatterName,
                $"Company name cannot be longer than {GameSettings.MaxCompanyNameLength} characters");
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(gameCommand.Argument)))
        {
            PublishMessage(chatter.ChatterName, 
                $"There is already a company named {gameCommand.Argument}");
            return;
        }

        chatter.Company.CompanyName = gameCommand.Argument;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.ChatterName,
            $"Your company is now named {chatter.Company.CompanyName}");
    }
}