using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public class HireCommandHandler : BaseCommandHandler
{
    public HireCommandHandler(GameSettings gameSettings,
        Dictionary<string, Company> players)
        : base("hire", gameSettings, players)
    {
    }

    public override void Execute(ChatCommand chatCommand)
    {
        var chatter = ChatterDetails(chatCommand);

        if(chatter.Company == null)
        {
            PublishMessage(chatter.Name, 
                Literals.YouDoNotHaveACompany);
            return;
        }

        if(chatCommand.ArgumentsAsList.Count != 2)
        {
            PublishMessage(chatter.Name,
                "To hire an employee, type '!hire <job name> <quantity>'");
            return;
        }

        var hiringDetails = GetHiringDetails(chatCommand.ArgumentsAsList);

        if(hiringDetails.Job == null || hiringDetails.Qty == null)
        {
            PublishMessage(chatter.Name,
                "To hire an employee, type '!hire <job name> <quantity>'");
            return;
        }

        if (hiringDetails.Qty < 1)
        {
            PublishMessage(chatter.Name,
                "Quantity must be greater than zero");
            return;
        }

        var empHiringDetails = 
            GameSettings.EmployeeHiringDetails
                .First(ehd => ehd.Type == hiringDetails.Job);

        int? hiringCost = empHiringDetails.CostToHire * hiringDetails.Qty;

        if (hiringCost > chatter.Company.Points)
        {
            PublishMessage(chatter.Name,
                $"It costs {hiringCost} {GameSettings.PointsName} to hire {hiringDetails.Qty} {hiringDetails.Job} employees. You only have {chatter.Company.Points} {GameSettings.PointsName}");
            return;
        }

        chatter.Company.Points -= (int)hiringCost;

        for(int i = 0; i < hiringDetails.Qty; i++)
        {
            chatter.Company.Employees
                .Add(new Employee 
                { 
                    Type = empHiringDetails.Type, 
                    SkillLevel = 1
                });
        }

        NotifyPlayerDataUpdated();
        PublishMessage(chatter.Name,
            $"Congratulations! You hired {hiringDetails.Qty} {hiringDetails.Job} employees");
    }

    private static (EmployeeType? Job, int? Qty) GetHiringDetails(List<string> arguments)
    {
        int? qty = null;
        EmployeeType? employeeType = null;

        if (int.TryParse(arguments[0], out int qty0))
        {
            qty = qty0;

            if (Enum.TryParse(arguments[1], true, out EmployeeType emp))
            {
                employeeType = emp;
            }
        }
        else if (int.TryParse(arguments[1], out int qty1))
        {
            qty = qty1;

            if (Enum.TryParse(arguments[0], true, out EmployeeType emp))
            {
                employeeType = emp;
            }
        }

        return (employeeType, qty);
    }
}