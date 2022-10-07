using CSharpExtender.Services;
using MegaCorpClash.Core;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    protected readonly ArgumentParser _argumentParser = new();

    public string CommandName { get; }
    protected GameSettings GameSettings { get; }
    protected Dictionary<string, Company> Companies { get; }

    protected string CompaniesList =>
        string.Join(", ",
            Companies.Values
                .OrderByDescending(c => c.Points)
                .Take(7)
                .Select(c => $"{c.CompanyName}: {c.Points:N0}"));

    public event EventHandler<ChatMessageEventArgs> OnChatMessageToSend;
    public event EventHandler OnPlayerDataUpdated;

    protected (string ChatterId, string ChatterName, Company? Company)
        ChatterDetails(GameCommandArgs gameCommand) =>
        (gameCommand.UserId,
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
        OnChatMessageToSend?.Invoke(this,
            new ChatMessageEventArgs("", message));
    }

    protected void PublishMessage(string chatterDisplayName, string message)
    {
        OnChatMessageToSend?.Invoke(this, 
            new ChatMessageEventArgs(chatterDisplayName, message));
    }

    protected void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
    }
}