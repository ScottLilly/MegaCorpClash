using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class StartCommandHandler : BaseCommandHandler
{
    public StartCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base("start", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        var chatter = ChatterDetails();

        if (GameCommandArgs.DoesNotHaveArguments)
        {
            PublishMessage(Literals.Start_NameRequired);
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

        if (chatter.Company != null)
        {
            PublishMessage($"You already have a company named {chatter.Company.CompanyName}");
            return;
        }

        if (CompanyCompanyRepository.OtherCompanyIsNamed(GameCommandArgs.UserId, 
            GameCommandArgs.Argument))
        {
            PublishMessage($"There is already a company named {GameCommandArgs.Argument}");
            return;
        }

        var chatterCompany =
            new Company
            {
                UserId = chatter.ChatterId,
                DisplayName = chatter.ChatterName,
                IsBroadcaster =
                    chatter.ChatterName.Matches(GameSettings.TwitchBroadcasterAccount.Name),
                CompanyName = GameCommandArgs.Argument,
                CreatedOn = DateTime.UtcNow,
                Points = GameSettings?.StartupDetails.InitialPoints ?? 0
            };

        GiveDefaultStaff(chatterCompany);

        CompanyCompanyRepository.AddCompany(chatterCompany);

        PublishMessage($"You are now the proud CEO of {GameCommandArgs.Argument}");
    }

    private void GiveDefaultStaff(Company company)
    {
        foreach (var staffDetails in GameSettings.StartupDetails.InitialStaff)
        {
            company.Employees
                .Add(new EmployeeQuantity
                {
                    Type = staffDetails.Type,
                    Quantity = staffDetails.Quantity
                });
        }
    }
}