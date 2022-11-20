using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class MottoCommandHandler : BaseCommandHandler
{
    public MottoCommandHandler(GameSettings gameSettings, 
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs) 
        : base("motto", gameSettings, companyCompanyRepository, gameCommandArgs)
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
            PublishMessage(Literals.SetMotto_MustProvideMotto);
            return;
        }

        if (GameCommandArgs.Argument.IsNotSafeText())
        {
            PublishMessage(Literals.SetMotto_NotSafeText);
            return;
        }

        if (GameCommandArgs.Argument.Length >
            GameSettings.MaxMottoLength)
        {
            PublishMessage($"Motto cannot be longer than {GameSettings.MaxMottoLength} characters");
            return;
        }

        CompanyCompanyRepository.ChangeMotto(chatter.ChatterId, GameCommandArgs.Argument);

        var updatedCompany = CompanyCompanyRepository.GetCompany(chatter.ChatterId);

        PublishMessage($"Your new company motto is '{updatedCompany.Motto}'");
    }
}