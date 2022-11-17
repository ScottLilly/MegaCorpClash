using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class RenameCommandHandler : BaseCommandHandler
{
    public RenameCommandHandler(GameSettings gameSettings,
        IRepository companyRepository)
        : base("rename", gameSettings, companyRepository)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

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
            PublishMessage(Literals.Start_NotSafeText);
            return;
        }

        if (gameCommandArgs.Argument.Length >
            GameSettings.MaxCompanyNameLength)
        {
            PublishMessage($"Company name cannot be longer than {GameSettings.MaxCompanyNameLength} characters");
            return;
        }

        if (CompanyRepository.OtherCompanyIsNamed(chatter.ChatterId, gameCommandArgs.Argument))
        {
            PublishMessage($"There is already a company named {gameCommandArgs.Argument}");
            return;
        }

        CompanyRepository.ChangeCompanyName(chatter.ChatterId, gameCommandArgs.Argument);

        var updatedCompany = CompanyRepository.GetCompany(chatter.ChatterId);

        PublishMessage($"Your company is now named {updatedCompany.CompanyName}");
    }
}