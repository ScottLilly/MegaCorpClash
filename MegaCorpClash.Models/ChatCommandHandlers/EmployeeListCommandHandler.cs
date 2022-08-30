using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class EmployeeListCommandHandler : BaseCommandHandler, IHandleChatCommand
{
    public EmployeeListCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("employees", gameSettings, players)
    {
    }

    public void Execute(ChatCommand chatCommand)
    {
        string chatterDisplayName = chatCommand.ChatterDisplayName();
        Player? player = GetPlayerObjectForChatter(chatCommand);

        if (player == null)
        {
            PublishMessage(chatterDisplayName, "You do not have a company");
            return;
        }

        string employeeList = 
            string.Join(", ", 
                player.Employees
                    .GroupBy(e => e.Type)
                    .Select(g => new
                    {
                        Job = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Job)
                    .Select(x => $"{x.Job} ({x.Count})"));

        PublishMessage(chatterDisplayName,
            player.Employees.Count == 1
                ? $"{player.CompanyName} has {player.Employees.Count} employee. {employeeList}"
                : $"{player.CompanyName} has {player.Employees.Count} employees. {employeeList}");
    }
}