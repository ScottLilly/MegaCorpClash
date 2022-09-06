namespace MegaCorpClash.Models.ChatCommandHandlers;

public class StaffCommandHandler : BaseCommandHandler
{
    public StaffCommandHandler(GameSettings gameSettings, 
        Dictionary<string, Company> companies)
        : base("staff", gameSettings, companies)
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

        string employeeList = 
            $"You have {chatter.Company.Employees.Count}" + 
            (chatter.Company.Employees.Count == 1 ? " employee. " : " employees. ") +
            chatter.Company.EmployeeList;

        PublishMessage(chatter.Name, employeeList);
    }
}