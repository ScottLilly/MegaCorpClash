using CSharpExtender.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class IncorporateCommandHandler : BaseCommandHandler
{
    public IncorporateCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("incorporate", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        var chatter = ChatterDetails(chatCommand);
        string? companyName = chatCommand.ArgumentsAsString;

        if (string.IsNullOrWhiteSpace(companyName))
        {
            PublishMessage(chatter.Name, 
                Literals.Incorporate_NameRequired);
            return;
        }

        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (player != null)
        {
            PublishMessage(chatter.Name,
                $"You already have a company named {player.CompanyName}");
            return;
        }

        if (Players.Values.Any(p => p.CompanyName.Matches(companyName)))
        {
            PublishMessage(chatter.Name,
                $"There is already a company named {companyName}");
            return;
        }

        player = new Player
        {
            Id = chatter.Id,
            DisplayName = chatter.Name,
            CompanyName = companyName,
            CreatedOn = DateTime.UtcNow,
            Employees = new List<Employee>
            {
                new() { Type = EmployeeType.Manufacturing, SkillLevel = 1 },
                new() { Type = EmployeeType.Sales, SkillLevel = 1 },
            }
        };

        Players[chatter.Id] = player;

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
            $"You are now the proud CEO of {companyName}");
    }
}