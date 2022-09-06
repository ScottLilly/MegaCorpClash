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
            PublishMessage(chatter.Name, Literals.YouDoNotHaveACompany);
            return;
        }

        if (string.IsNullOrWhiteSpace(gameCommand.Argument))
        {
            PublishMessage(chatter.Name, 
                "You must enter a value for the motto");
            return;
        }

        chatter.Company.Motto = gameCommand.Argument;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
                $"Your new company motto is '{chatter.Company.Motto}'");
    }
}