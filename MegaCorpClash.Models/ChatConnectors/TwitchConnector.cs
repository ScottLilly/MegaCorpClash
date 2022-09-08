using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace MegaCorpClash.Models.ChatConnectors;

public class TwitchConnector : IChatConnector
{
    private bool _hasConnected;

    private readonly string _channelName;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchClient _client = new();

    public event EventHandler OnConnected;
    public event EventHandler OnDisconnected;
    public event EventHandler<ChattedEventArgs> OnPersonChatted;
    public event EventHandler<GameCommand> OnGameCommandReceived;
    public event EventHandler<string> OnLogMessagePublished;

    public TwitchConnector(GameSettings gameSettings)
    {
        if (gameSettings == null)
        {
            throw new ArgumentNullException(nameof(gameSettings));
        }

        if (string.IsNullOrWhiteSpace(gameSettings.ChannelName))
        {
            throw new ArgumentException("ChannelName cannot be empty");
        }

        _channelName = gameSettings.ChannelName;
        _credentials = CreateCredentials(gameSettings);

        SubscribeToEvents();
    }

    #region Public functions

    public void Connect()
    {
        try
        {
            if (_hasConnected)
            {
                _client.Reconnect();
            }
            else
            {
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
            string.IsNullOrWhiteSpace(gameSettings.BotAccountName)
                ? gameSettings.ChannelName
                : gameSettings.BotAccountName,
            gameSettings.TwitchToken,
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
        OnDisconnected?.Invoke(this, EventArgs.Empty);

        Connect();
    }

    private void HandleChatMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        OnPersonChatted?.Invoke(this,
            new ChattedEventArgs(e.ChatMessage.UserId));
    }

    private void HandleChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        OnGameCommandReceived?.Invoke(this,
            new GameCommand(
                e.Command.ChatMessage.UserId,
                e.Command.ChatMessage.DisplayName,
                e.Command.CommandText,
                e.Command.ArgumentsAsString));
    }

    #endregion

    #region Private helper functions

    private void WriteMessageToLog(string message)
    {
        OnLogMessagePublished.Invoke(this, message);
    }

    #endregion
}