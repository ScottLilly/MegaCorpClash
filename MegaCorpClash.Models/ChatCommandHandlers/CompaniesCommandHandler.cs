using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class CompaniesCommandHandler : BaseCommandHandler
{
    public CompaniesCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("companies", gameSettings, companies)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        string playerList =
            string.Join(", ",
                Companies.Values
                    .OrderBy(c => c.CompanyName)
                    .Select(c => $"{c.CompanyName} ({c.ChatterName})"));

        if (string.IsNullOrWhiteSpace(playerList))
        {
            playerList = "No companies";
        }

        PublishMessage($"Companies: {playerList}");
    }
}