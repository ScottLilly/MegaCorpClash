﻿using MegaCorpClash.Models.CustomEventArgs;
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

    public event EventHandler<TwitchChatMessageEventArgs> OnMessagePublished;
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

    protected void PublishMessage(string chatterDisplayName, string message)
    {
        OnMessagePublished.Invoke(this, 
            new TwitchChatMessageEventArgs(chatterDisplayName, message));
    }

    protected void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
    }
}