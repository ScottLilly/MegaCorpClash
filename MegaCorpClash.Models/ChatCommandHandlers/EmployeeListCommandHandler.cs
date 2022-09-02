using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class EmployeeListCommandHandler : BaseCommandHandler
{
    public EmployeeListCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Player> players)
        : base("employees", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        var chatter = ChatterDetails(chatCommand);

        if (chatter.Player == null)
        {
            PublishMessage(chatter.Name, Literals.YouDoNotHaveACompany);
            return;
        }

        string message = 
            chatter.Player.CompanyName + 
            " has " + 
            chatter.Player.Employees.Count +
            (chatter.Player.Employees.Count == 1 ? " employee. " : " employees. ") +
            chatter.Player.EmployeeList;

        PublishMessage(chatter.Name, message);
    }
}