using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class RenameCommandHandler : BaseCommandHandler
{
    public RenameCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base("rename", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        var chatter = ChatterDetails();

        if (chatter.Company == null)
        {
            PublishMessage(Literals.YouDoNotHaveACompany);
            return;
        }

        if (GameCommandArgs.DoesNotHaveArguments)
        {
            PublishMessage(Literals.Rename_YouMustProvideANewName);
            return;
        }

        if (GameCommandArgs.Argument.IsNotSafeText())
        {
            PublishMessage(Literals.Start_NotSafeText);
            return;
        }

        if (GameCommandArgs.Argument.Length >
            GameSettings.MaxCompanyNameLength)
        {
            PublishMessage($"Company name cannot be longer than {GameSettings.MaxCompanyNameLength} characters");
            return;
        }

        if (CompanyCompanyRepository.OtherCompanyIsNamed(chatter.ChatterId, GameCommandArgs.Argument))
        {
            PublishMessage($"There is already a company named {GameCommandArgs.Argument}");
            return;
        }

        CompanyCompanyRepository.ChangeCompanyName(chatter.ChatterId, GameCommandArgs.Argument);

        var updatedCompany = CompanyCompanyRepository.GetCompany(chatter.ChatterId);

        PublishMessage($"Your company is now named {updatedCompany.CompanyName}");
    }
}