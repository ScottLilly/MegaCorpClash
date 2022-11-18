using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;

namespace MegaCorpClash.Services.ChatCommandHandlers;

public sealed class StaffCommandHandler : BaseCommandHandler
{
    public StaffCommandHandler(GameSettings gameSettings,
        IRepository companyRepository, GameCommandArgs gameCommandArgs)
        : base("staff", gameSettings, companyRepository, gameCommandArgs)
    {
    }

    public override void Execute()
    {
        LogTraceMessage();

        var chatter = ChatterDetails();

        if (chatter.Company == null)
        {
            PublishMessage(Literals.YouDoNotHaveACompany);
            return;
        }

        string employeeList =
            $"You have {chatter.Company.Employees.Sum(e => e.Quantity):N0}" +
            (chatter.Company.Employees.Count == 1 ? " employee. " : " employees. ") +
            chatter.Company.EmployeeList;

        PublishMessage(employeeList);
    }
}