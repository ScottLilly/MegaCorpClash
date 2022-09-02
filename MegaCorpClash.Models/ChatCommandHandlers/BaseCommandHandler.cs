using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    public string CommandName { get; }
    protected GameSettings GameSettings { get; }
    protected Dictionary<string, Company> Companies { get; }

    public event EventHandler<ChatMessageEventArgs> OnChatMessagePublished;
    public event EventHandler OnPlayerDataUpdated;

    protected (string Id, string Name, Company? Company) 
        ChatterDetails(ChatCommand chatCommand) =>
        (chatCommand.ChatMessage.UserId, 
            chatCommand.ChatMessage.DisplayName, 
            Companies.ContainsKey(chatCommand.ChatMessage.UserId) 
                ? Companies[chatCommand.ChatMessage.UserId] 
                : null);

    protected BaseCommandHandler(string commandName, GameSettings gameSettings, 
        Dictionary<string, Company> companies)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        Companies = companies;
    }

    public abstract void Execute(ChatCommand chatCommand);

    protected void PublishMessage(string message)
    {
        OnChatMessagePublished?.Invoke(this,
            new ChatMessageEventArgs("", message));
    }

    protected void PublishMessage(string chatterDisplayName, string message)
    {
        OnChatMessagePublished?.Invoke(this, 
            new ChatMessageEventArgs(chatterDisplayName, message));
    }

    protected void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
    }
}