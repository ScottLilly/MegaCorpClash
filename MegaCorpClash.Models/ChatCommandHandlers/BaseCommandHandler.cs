using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    public string CommandName { get; }
    protected GameSettings GameSettings { get; }
    protected Dictionary<string, Company> Players { get; }

    public event EventHandler<ChatMessageEventArgs> OnChatMessagePublished;
    public event EventHandler OnPlayerDataUpdated;

    protected (string Id, string Name, Company? Company) ChatterDetails(ChatCommand chatCommand) =>
        (chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName, Players[chatCommand.ChatMessage.UserId]);

    protected BaseCommandHandler(string commandName, GameSettings gameSettings, 
        Dictionary<string, Company> players)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        Players = players;
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