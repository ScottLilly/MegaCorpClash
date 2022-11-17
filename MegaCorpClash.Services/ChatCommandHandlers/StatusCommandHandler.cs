using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class StatusCommandHandler : BaseCommandHandler
{
    public StatusCommandHandler(GameSettings gameSettings,
        IRepository companyRepository)
        : base("status", gameSettings, companyRepository)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        LogTraceMessage();

        var chatter = ChatterDetails(gameCommandArgs);

        PublishMessage(chatter.Company == null
                ? Literals.YouDoNotHaveACompany
                : $"At {chatter.Company.CompanyName} we always say '{chatter.Company.Motto}'. That's how we earned {chatter.Company.Points:N0} {GameSettings.PointsName}");
    }
}