using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HireCommandHandler : BaseCommandHandler
{
    public HireCommandHandler(GameSettings gameSettings,
        Dictionary<string, Player> players)
        : base("hire", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        Player? player = GetPlayerObjectForChatter(chatCommand);

        if(player == null)
        {
            PublishMessage(chatCommand.ChatterDisplayName(), 
                "You do not have a company");
            return;
        }

        if(chatCommand.ArgumentsAsList.Count != 2)
        {
            PublishMessage(chatCommand.ChatterDisplayName(),
                "To hire an employee, type '!hire <employee type> <quantity>'");
            return;
        }

        int? qty = null;
        EmployeeType? employeeType = null;

        if (int.TryParse(chatCommand.ArgumentsAsList[0], out int qty0))
        {
            qty = qty0;

            if (Enum.TryParse(chatCommand.ArgumentsAsList[1], true, out EmployeeType emp))
            {
                employeeType = emp;
            }
        }
        else if (int.TryParse(chatCommand.ArgumentsAsList[1], out int qty1))
        {
            qty = qty1;

            if (Enum.TryParse(chatCommand.ArgumentsAsList[0], true, out EmployeeType emp))
            {
                employeeType = emp;
            }
        }

        if(qty == null || employeeType == null)
        {
            PublishMessage(chatCommand.ChatterDisplayName(),
                "To hire an employee, type '!hire <employee type> <quantity>'");
            return;
        }

        if (qty < 1)
        {
            PublishMessage(chatCommand.ChatterDisplayName(),
                "Quantity of employees to hire must be greater than zero");
            return;
        }

        var empHiringDetails = 
            GameSettings.EmployeeHiringDetails
            .First(ehd => ehd.Type == employeeType);

        if(empHiringDetails == null)
        {
            PublishMessage(chatCommand.ChatterDisplayName(),
                $"MegaCorpClash does not recognize the employee type: {employeeType}");
            return;
        }

        int? hiringCost = empHiringDetails.CostToHire * qty;

        if (hiringCost > player.Points)
        {
            PublishMessage(chatCommand.ChatterDisplayName(),
                $"It would cost {hiringCost} {GameSettings.PointsName} to hire {qty} {employeeType} employees. You only have {player.Points} {GameSettings.PointsName}");
            return;
        }

        player.Points -= (int)hiringCost;
        for(int i = 0; i < qty; i++)
        {
            player.Employees.Add(new Employee 
            { 
                Type = empHiringDetails.Type, 
                SkillLevel = 1 
            });
        }

        NotifyPlayerDataUpdated();
        PublishMessage(chatCommand.ChatterDisplayName(),
            $"Congratulations! You hired {qty} {employeeType} employees");
    }
}