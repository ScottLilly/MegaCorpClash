using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class StatusCommandHandler : BaseCommandHandler
{
    public StatusCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("status", gameSettings, companies)
    {
    }

    public override void Execute(GameCommandArgs gameCommandArgs)
    {
        var chatter = ChatterDetails(gameCommandArgs);

        PublishMessage(chatter.Company == null
                ? Literals.YouDoNotHaveACompany
                : $"At {chatter.Company.CompanyName} we always say '{chatter.Company.Motto}'. That's how we earned {chatter.Company.Points:N0} {GameSettings.PointsName}");
    }
}