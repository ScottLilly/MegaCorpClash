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

        if (string.IsNullOrWhiteSpace(chatCommand.ArgumentsAsString))
        {
            PublishMessage(chatter.Name, 
                "You must enter a value for the motto");
            return;
        }

        if (chatter.Company == null)
        {
            PublishMessage(chatter.Name, Literals.YouDoNotHaveACompany);
            return;
        }

        chatter.Company.Motto = chatCommand.ArgumentsAsString;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
                $"Your new company motto is '{chatter.Company.Motto}'");
    }
}