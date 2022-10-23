using CSharpExtender.Services;
using MegaCorpClash.Core;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    protected readonly ArgumentParser _argumentParser = new();

    public string CommandName { get; }
    public bool BroadcasterCanRun { get; protected init; } = true;
    public bool NonBroadcasterCanRun { get; protected init; } = true;
    protected GameSettings GameSettings { get; }
    protected Dictionary<string, Company> Companies { get; }

    protected string TopCompaniesByPoints =>
        string.Join(", ",
            Companies.Values
                .OrderByDescending(c => c.Points)
                .Take(7)
                .Select(c => $"{c.CompanyName} [{c.Points:N0}]"));

    public List<string> ChatMessages { get; } = new();
    public bool PlayerDataUpdated { get; private set; } = false;
    public bool StreamerBankrupted { get; private set; } = false;

    public ChatterDetails
        ChatterDetails(GameCommandArgs gameCommand) =>
        new ChatterDetails(gameCommand.UserId,
            gameCommand.DisplayName,
            Companies.ContainsKey(gameCommand.UserId)
            ? Companies[gameCommand.UserId]
            : null);

    protected Company GetBroadcasterCompany =>
        Companies.First(c => c.Value.IsBroadcaster).Value;

    protected BaseCommandHandler(string commandName, GameSettings gameSettings, 
        Dictionary<string, Company> companies)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        Companies = companies;
    }

    public abstract void Execute(GameCommandArgs gameCommandArgs);

    protected bool IsAttackSuccessful(EmployeeType guardEmployeeType)
    {
        var broadcasterCompany = GetBroadcasterCompany;

        int securityEmployeeCount =
            broadcasterCompany.Employees
                .Where(e => e.Type == guardEmployeeType)
                .Sum(e => e.Quantity)
            + 1;

        int broadcasterDefenseBonus =
            Math.Min(25, Convert.ToInt32(Math.Log10(securityEmployeeCount) * 10));

        int rand = RngCreator.GetNumberBetween(1, 100);

        return rand > 50 + broadcasterDefenseBonus;
    }

    protected void PublishMessage(string message)
    {
        ChatMessages.Add(message);
    }

    protected void NotifyPlayerDataUpdated()
    {
        PlayerDataUpdated = true;
    }

    protected void NotifyBankruptedStreamer()
    {
        StreamerBankrupted = true;
    }
}