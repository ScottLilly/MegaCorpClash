using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    protected readonly Dictionary<string, Player> _players;

    protected static string TwitchUserId(ChatCommand chatCommand) => 
        chatCommand.ChatMessage.UserId;
    protected static string DisplayName(ChatCommand chatCommand) => 
        chatCommand.ChatMessage.DisplayName;
    protected static string Arguments(ChatCommand chatCommand) => 
        chatCommand.ArgumentsAsString;

    public event EventHandler<MessageEventArgs> OnMessagePublished;
    public event EventHandler OnPlayerDataUpdated;

    protected BaseCommandHandler(Dictionary<string, Player> players)
    {
        _players = players;
    }

    protected Player? GetPlayerObjectForChatter(ChatCommand chatCommand)
    {
        _players.TryGetValue(TwitchUserId(chatCommand), out Player? player);

        return player;
    }

    protected void PublishMessage(ChatCommand chatCommand, string message)
    {
        OnMessagePublished.Invoke(this, new MessageEventArgs(chatCommand, message));
    }

    protected void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
    }
}