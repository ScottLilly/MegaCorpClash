namespace MegaCorpClash.Models.ChatCommandHandlers;

public class StatusCommandHandler : BaseCommandHandler
{
    public StatusCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("status", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        PublishMessage(chatter.ChatterName,
            chatter.Company == null
                ? Literals.YouDoNotHaveACompany
                : $"At {chatter.Company.CompanyName} we always say '{chatter.Company.Motto}'. That's how we earned {chatter.Company.Points} {GameSettings.PointsName}");
    }
}