using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client.Models;

namespace MegaCorpClash.Models.ChatCommandHandlers;

public abstract class BaseCommandHandler
{
    protected readonly Dictionary<string, Player> _players;

    protected string TwitchUserId(ChatCommand chatCommand) => 
        chatCommand.ChatMessage.UserId;
    protected string DisplayName(ChatCommand chatCommand) => 
        chatCommand.ChatMessage.DisplayName;
    protected string Arguments(ChatCommand chatCommand) => 
        chatCommand.ArgumentsAsString;

    public event EventHandler<MessageEventArgs> OnMessageToDisplay;
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

    protected void InvokeMessageToDisplay(ChatCommand chatCommand, 
        string message, bool displayInTwitchChat = true)
    {
        OnMessageToDisplay
            .Invoke(this, 
                new MessageEventArgs(chatCommand, message, displayInTwitchChat));
    }

    protected void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
    }
}