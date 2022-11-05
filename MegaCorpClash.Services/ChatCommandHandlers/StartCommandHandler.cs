using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Core;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class StartCommandHandler : BaseCommandHandler
{
    public StartCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("start", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

        var chatter = ChatterDetails(gameCommandArgs);

        if (gameCommandArgs.DoesNotHaveArguments)
        {
            PublishMessage(Literals.Start_NameRequired);
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

        if (chatter.Company != null)
        {
            PublishMessage($"You already have a company named {chatter.Company.CompanyName}");
            return;
        }

        if (Companies.Values.Any(p => p.CompanyName.Matches(gameCommandArgs.Argument)))
        {
            PublishMessage($"There is already a company named {gameCommandArgs.Argument}");
            return;
        }

        chatter.Company =
            new Company
            {
                UserId = chatter.ChatterId,
                DisplayName = chatter.ChatterName,
                IsBroadcaster =
                    chatter.ChatterName.Matches(GameSettings.TwitchBroadcasterAccount.Name),
                CompanyName = gameCommandArgs.Argument,
                CreatedOn = DateTime.UtcNow,
                Points = GameSettings?.StartupDetails.InitialPoints ?? 0
            };

        GiveDefaultStaff(chatter.Company);

        Companies[chatter.ChatterId] = chatter.Company;

        NotifyCompanyDataUpdated();
        PublishMessage($"You are now the proud CEO of {gameCommandArgs.Argument}");
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