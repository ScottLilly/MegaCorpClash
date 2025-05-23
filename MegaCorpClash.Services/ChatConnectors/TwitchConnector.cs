﻿using MegaCorpClash.Models;
using MegaCorpClash.Services.CustomEventArgs;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace MegaCorpClash.Services.ChatConnectors;

public sealed class TwitchConnector : IChatConnector
{
    private static readonly NLog.Logger Logger =
        NLog.LogManager.GetCurrentClassLogger();

    private bool _hasConnected;

    private readonly string _channelName;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchClient _client = new();

    public event EventHandler OnConnected;
    public event EventHandler OnDisconnected;
    public event EventHandler<ChattedEventArgs> OnPersonChatted;
    public event EventHandler<GameCommandArgs> OnGameCommandReceived;
    public event EventHandler<LogMessageEventArgs> OnLogMessagePublished;

    public TwitchConnector(GameSettings gameSettings)
    {
        if (gameSettings == null)
        {
            throw new ArgumentNullException(nameof(gameSettings));
        }

        if (gameSettings.TwitchBroadcasterAccount == null)
        {
            throw new ArgumentNullException(nameof(gameSettings.TwitchBroadcasterAccount));
        }

        if (gameSettings.TwitchBotAccount == null)
        {
            throw new ArgumentNullException(nameof(gameSettings.TwitchBotAccount));
        }

        if (string.IsNullOrWhiteSpace(gameSettings.TwitchBroadcasterAccount.Name))
        {
            throw new ArgumentException("ChannelName cannot be empty");
        }

        _channelName = gameSettings.TwitchBroadcasterAccount.Name;
        _credentials = CreateCredentials(gameSettings);

        Logger.Debug("Subscribing to Twitch events");

        SubscribeToEvents();
    }

    #region Public functions

    public void Connect()
    {
        try
        {
            if (_hasConnected)
            {
                Logger.Debug("Reconnecting to Twitch");
                _client.Reconnect();
            }
            else
            {
                Logger.Debug("Connecting to Twitch");
                _client.Initialize(_credentials, _channelName);
                _client.Connect();
                _hasConnected = true;
            }
        }
        catch (Exception e)
        {
            WriteMessageToLog($"Exception during Connect(): {e.Message}");

            throw;
        }
    }

    public void Disconnect()
    {
        Logger.Trace("Disconnecting from Twitch");
        UnsubscribeFromEvents();

        _client?.Disconnect();
    }

    public void SendChatMessage(string? message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            _client.SendMessage(_channelName, message);
        }
    }

    #endregion

    #region Private Twitch connection functions

    private static ConnectionCredentials CreateCredentials(GameSettings gameSettings)
    {
        return new ConnectionCredentials(
            string.IsNullOrWhiteSpace(gameSettings.TwitchBotAccount?.Name)
                ? gameSettings.TwitchBroadcasterAccount?.Name
                : gameSettings.TwitchBotAccount?.Name,
            gameSettings.TwitchBotAccount?.AuthToken,
            disableUsernameCheck: true);
    }

    private void SubscribeToEvents()
    {
        _client.OnConnected += HandleConnected;
        _client.OnDisconnected += HandleDisconnected;
        _client.OnMessageReceived += HandleChatMessageReceived;
        _client.OnChatCommandReceived += HandleChatCommandReceived;
    }

    private void UnsubscribeFromEvents()
    {
        _client.OnConnected -= HandleConnected;
        _client.OnDisconnected -= HandleDisconnected;
        _client.OnMessageReceived -= HandleChatMessageReceived;
        _client.OnChatCommandReceived -= HandleChatCommandReceived;
    }

    #endregion

    #region Private event handler functions

    private void HandleConnected(object? sender, OnConnectedArgs e)
    {
        OnConnected?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDisconnected(object? sender, OnDisconnectedEventArgs e)
    {
        Logger.Debug("Disconnected from Twitch");
        OnDisconnected?.Invoke(this, EventArgs.Empty);

        Connect();
    }

    private void HandleChatMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        OnPersonChatted?.Invoke(this,
            new ChattedEventArgs(
                e.ChatMessage.UserId,
                e.ChatMessage.DisplayName,
                e.ChatMessage.IsBroadcaster,
                e.ChatMessage.IsSubscriber,
                e.ChatMessage.IsVip,
                e.ChatMessage.Noisy == Noisy.True));
    }

    private void HandleChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        OnGameCommandReceived?.Invoke(this,
            new GameCommandArgs(
                e.Command.ChatMessage.UserId,
                e.Command.ChatMessage.DisplayName,
                e.Command.CommandText,
                e.Command.ArgumentsAsString,
                e.Command.ChatMessage.IsBroadcaster,
                e.Command.ChatMessage.IsSubscriber,
                e.Command.ChatMessage.IsVip,
                e.Command.ChatMessage.Noisy == Noisy.True));
    }

    #endregion

    #region Private helper functions

    private void WriteMessageToLog(string message)
    {
        OnLogMessagePublished.Invoke(this, new LogMessageEventArgs(message));
    }

    #endregion
}