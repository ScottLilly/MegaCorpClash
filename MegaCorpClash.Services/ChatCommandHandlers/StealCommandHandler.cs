using CSharpExtender.ExtensionMethods;
using CSharpExtender.Services;
using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;
using Newtonsoft.Json.Linq;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public class StealCommandHandler : BaseCommandHandler
{
    private GameSettings.AttackDetail _attackDetail;

    public StealCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("steal", gameSettings, companyRepository, gameCommandArgs)
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

        int numberOfAttackingSpies =
            GetNumberOfAttackingSpies(GameCommandArgs, chatter.Company);

        if (numberOfAttackingSpies < 1)
        {
            PublishMessage("Number of attacking spies must be greater than 0");
            return;
        }

        // Check if player's company has enough spies
        int availableSpies = 
            chatter.Company.Employees
            .FirstOrDefault(e => e.Type == EmployeeType.Spy)?.Quantity ?? 0;

        if (availableSpies < numberOfAttackingSpies)
        {
            if(availableSpies == 0)
            {
                PublishMessage("You don't have any spies");
            }
            else if(availableSpies == 1)
            {
                PublishMessage("You only have 1 spy");
            }
            else
            {
                PublishMessage($"You only have {availableSpies} spies");
            }
            return;
        }

        int broadcasterPoints = GetBroadcasterCompany.Points;
        int successCount = 0;
        long totalPointsStolen = 0;
        int securityPeopleLost = 0;

        for (int i = 0; i < numberOfAttackingSpies; i++)
        {
            var attackSuccessful = IsAttackSuccessful(EmployeeType.Security);

            if (attackSuccessful)
            {
                // Success
                int stolen = 
                    broadcasterPoints / 
                    RngCreator.GetNumberBetween(_attackDetail.Min, _attackDetail.Max);

                if (broadcasterPoints < 1000)
                {
                    stolen = broadcasterPoints;
                }

                successCount++;
                totalPointsStolen += stolen;
                broadcasterPoints -= stolen;
            }
            else
            {
                // Failure, consumes broadcaster security person
                securityPeopleLost++;
            }
        }

        // Do debits/credits
        // "Consume" spies used during attack
        CompanyRepository.RemoveEmployeeOfType(chatter.ChatterId, EmployeeType.Spy, numberOfAttackingSpies);
        CompanyRepository.AddPoints(chatter.ChatterId, (int)totalPointsStolen);

        CompanyRepository.RemoveEmployeeOfType(GetBroadcasterCompany.UserId, EmployeeType.Security, securityPeopleLost);
        CompanyRepository.SubtractPoints(GetBroadcasterCompany.UserId, (int)totalPointsStolen);

        // Get correct chatter company
        var updatedCompany = CompanyRepository.GetCompany(chatter.ChatterId);

        // Display messages
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
            CompanyRepository.IncrementVictoryCount(chatter.ChatterId);
            NotifyBankruptedStreamer();
        }
    }
}