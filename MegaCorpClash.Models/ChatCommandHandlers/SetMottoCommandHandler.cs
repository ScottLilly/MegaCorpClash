using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class SetMottoCommandHandler : BaseCommandHandler
{
    public SetMottoCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> companies)
        : base("setmotto", gameSettings, companies)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        var chatter = ChatterDetails(chatCommand);

        if (chatter.Company == null)
        {
            PublishMessage(chatter.Name, Literals.YouDoNotHaveACompany);
            return;
        }

        if (string.IsNullOrWhiteSpace(chatCommand.ArgumentsAsString))
        {
            PublishMessage(chatter.Name, 
                "You must enter a value for the motto");
            return;
        }

        chatter.Company.Motto = chatCommand.ArgumentsAsString;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
                $"Your new company motto is '{chatter.Company.Motto}'");
    }
}