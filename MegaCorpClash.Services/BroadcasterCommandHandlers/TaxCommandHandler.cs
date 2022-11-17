using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.BroadcasterCommandHandlers;

public class TaxCommandHandler : BroadcasterOnlyCommandHandler
{
    public TaxCommandHandler(GameSettings gameSettings,
        IRepository companyRepository)
        : base("tax", gameSettings, companyRepository)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

        if (!gameCommandArgs.IsBroadcaster)
        {
            return;
        }

        var parsedArguments =
            _argumentParser.Parse(gameCommandArgs.Argument);

        if (parsedArguments.IntegerArguments.Count != 1 ||
            parsedArguments.IntegerArguments.First() < 1 ||
            parsedArguments.IntegerArguments.First() > 99)
        {
            PublishMessage("Tax command requires a single integer tax rate between 1 and 99");
            return;
        }

        Logger.Trace($"Applying tax of {parsedArguments.IntegerArguments.First()}%");

        var taxPercentage = parsedArguments.IntegerArguments.First() / 100M;

        foreach (var company in CompanyRepository.GetAllCompanies()
            .Where(c => !c.IsBroadcaster))
        {
            var amountToRemove = Convert.ToInt32(company.Points * taxPercentage);
            CompanyRepository.SubtractPoints(company.UserId, amountToRemove);
        }

        PublishMessage($"A {parsedArguments.IntegerArguments.First()}% tax was applied to all companies");
    }
}