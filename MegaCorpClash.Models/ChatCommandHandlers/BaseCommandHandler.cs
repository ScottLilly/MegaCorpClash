using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Models.ExtensionMethods;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    protected readonly Dictionary<string, Player> _players;

    public string CommandName { get; }
    protected GameSettings GameSettings { get; }


    public event EventHandler<ChatMessageEventArgs> OnChatMessagePublished;
    public event EventHandler OnPlayerDataUpdated;

    protected BaseCommandHandler(string commandName, GameSettings gameSettings, 
        Dictionary<string, Player> players)
    {
        CommandName = commandName;
        GameSettings = gameSettings;
        _players = players;
    }

    protected Player? GetPlayerObjectForChatter(ChatCommand chatCommand)
    {
        _players.TryGetValue(chatCommand.ChatterUserId(), out Player? player);

        return player;
    }

    protected void PublishMessage(string chatterDisplayName, string message)
    {
        OnChatMessagePublished.Invoke(this, 
            new ChatMessageEventArgs(chatterDisplayName, message));
    }

    protected void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
    }
}