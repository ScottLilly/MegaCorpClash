using MegaCorpClash.Core;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class SetMottoCommandHandler : BaseCommandHandler
{
    public SetMottoCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("setmotto", gameSettings, companies)
    {
    }

    public override void Execute(GameCommand gameCommand)
    {
        var chatter = ChatterDetails(gameCommand);

        if (chatter.Company == null)
        {
            PublishMessage(chatter.ChatterName, Literals.YouDoNotHaveACompany);
            return;
        }

        if (string.IsNullOrWhiteSpace(gameCommand.Argument))
        {
            PublishMessage(chatter.ChatterName, 
                "You must enter a value for the motto");
            return;
        }

        if (!gameCommand.Argument.IsSafeText())
        {
            PublishMessage(chatter.ChatterName,
                Literals.CompanyMotto_NotSafeText);
            return;
        }

        chatter.Company.Motto = gameCommand.Argument;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.ChatterName,
                $"Your new company motto is '{chatter.Company.Motto}'");
    }
}