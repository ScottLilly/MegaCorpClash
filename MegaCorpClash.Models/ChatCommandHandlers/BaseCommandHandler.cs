using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    public string CommandName { get; }
    protected GameSettings GameSettings { get; }
    protected Dictionary<string, Player> Players { get; }

    public event EventHandler<ChatMessageEventArgs> OnChatMessagePublished;
    public event EventHandler OnPlayerDataUpdated;

    protected (string Id, string Name, Player? Player) ChatterDetails(ChatCommand chatCommand) =>
        (chatCommand.ChatMessage.UserId, chatCommand.ChatMessage.DisplayName, Players[chatCommand.ChatMessage.UserId]);

    protected BaseCommandHandler(string commandName, GameSettings gameSettings, 
        Dictionary<string, Player> players)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        Players = players;
    }

    public abstract void Execute(ChatCommand chatCommand);

    protected Player? GetPlayerObjectForChatter(ChatCommand chatCommand)
    {
        Players.TryGetValue(chatCommand.ChatterUserId(), out Player? player);

        return player;
    }

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