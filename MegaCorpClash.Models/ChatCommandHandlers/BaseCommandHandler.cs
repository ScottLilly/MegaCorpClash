using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
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
        ChatterDetails(GameCommand gameCommand) =>
        (gameCommand.UserId,
            gameCommand.DisplayName,
            Companies.ContainsKey(gameCommand.UserId)
                ? Companies[gameCommand.UserId]
                : null);

    protected BaseCommandHandler(string commandName, GameSettings gameSettings, 
        Dictionary<string, Company> companies)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        Companies = companies;
    }

    public abstract void Execute(GameCommand gameCommand);

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