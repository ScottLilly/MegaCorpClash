using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class RenameCommandHandler : BaseCommandHandler
{
    public RenameCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("rename", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        var chatter = ChatterDetails(gameCommandArgs);

        if (chatter.Company == null)
        {
            PublishMessage(Literals.YouDoNotHaveACompany);
            return;
        }

        if (gameCommandArgs.DoesNotHaveArguments)
        {
            PublishMessage(Literals.Rename_YouMustProvideANewName);
            return;
        }

        if (gameCommandArgs.Argument.IsNotSafeText())
        {
            PublishMessage(Literals.Incorporate_NotSafeText);
            return;
        }

        if (gameCommandArgs.Argument.Length >
            GameSettings.MaxCompanyNameLength)
        {
            PublishMessage($"Company name cannot be longer than {GameSettings.MaxCompanyNameLength} characters");
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(gameCommandArgs.Argument)))
        {
            PublishMessage($"There is already a company named {gameCommandArgs.Argument}");
            return;
        }

        chatter.Company.CompanyName = gameCommandArgs.Argument;

        NotifyPlayerDataUpdated();
        PublishMessage($"Your company is now named {chatter.Company.CompanyName}");
    }
}