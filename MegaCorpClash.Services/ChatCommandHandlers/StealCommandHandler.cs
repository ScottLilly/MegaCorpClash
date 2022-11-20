using CSharpExtender.ExtensionMethods;
using CSharpExtender.Services;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class StealCommandHandler : BaseCommandHandler
{
    private GameSettings.AttackDetail _attackDetail;

    public StealCommandHandler(GameSettings gameSettings,
        ICompanyRepository companyCompanyRepository, GameCommandArgs gameCommandArgs)
        : base("steal", gameSettings, companyCompanyRepository, gameCommandArgs)
    {
        BroadcasterCanRun = false;
        _attackDetail = 
            GameSettings.AttackDetails?
            .FirstOrDefault(ad => ad.AttackType.Matches(CommandName))
            ?? new GameSettings.AttackDetail { Min = 100, Max = 500 };
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

        // Check if player's company has a Spy
        if (chatter.Company.Employees.None(e => e.Type == EmployeeType.Spy))
        {
            PublishMessage("You must have at least one spy to steal");
            return;
        }

        int numberOfAttackingSpies =
            GetNumberOfAttackingSpies(GameCommandArgs, chatter.Company);

        if (numberOfAttackingSpies < 1)
        {
            PublishMessage("Number of attacking spies must be greater than 0");
            return;
        }

        int successCount = 0;
        long totalPointsStolen = 0;

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            // "Consume" spy during attack
            CompanyCompanyRepository.RemoveEmployeeOfType(chatter.ChatterId, EmployeeType.Spy);

            var attackSuccessful = IsAttackSuccessful(EmployeeType.Security);

            if (attackSuccessful)
            {
                // Success
                int stolen = 
                    GetBroadcasterCompany.Points / 
                    RngCreator.GetNumberBetween(_attackDetail.Min, _attackDetail.Max);

                if (GetBroadcasterCompany.Points < 1000)
                {
                    stolen = GetBroadcasterCompany.Points;
                }

                CompanyCompanyRepository.AddPoints(chatter.ChatterId, stolen);
                CompanyCompanyRepository.SubtractPoints(GetBroadcasterCompany.UserId, stolen);

                successCount++;
                totalPointsStolen += stolen;
            }
            else
            {
                // Failure, consumes broadcaster security person
                CompanyCompanyRepository.RemoveEmployeeOfType(GetBroadcasterCompany.UserId, EmployeeType.Security); 
            }
        }

        var updatedCompany = CompanyCompanyRepository.GetCompany(chatter.ChatterId);

        if (numberOfAttackingSpies == 1)
        {
            PublishMessage(successCount == 1
                    ? $"Your spy stole {totalPointsStolen:N0} {GameSettings.PointsName} and now have {updatedCompany.Points:N0} {GameSettings.PointsName}"
                    : "Your spy was caught and you got nothing");
        }
        else
        {
            PublishMessage($"You had {successCount:N0}/{numberOfAttackingSpies:N0} successful attacks and stole {totalPointsStolen:N0} {GameSettings.PointsName} and now have {updatedCompany.Points:N0} {GameSettings.PointsName}");
        }

        if (GetBroadcasterCompany.Points == 0)
        {
            NotifyBankruptedStreamer();
        }
    }
}