using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models.ChatCommandHandlers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace MegaCorpClash.Models;

public class TwitchConnector
{
    private bool _hasConnected;

    private readonly string _channelName;
    private readonly List<IHandleChatCommand> _chatCommandHandlers;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchClient _client = new();

    public event EventHandler OnConnected;
    public event EventHandler OnDisconnected; 
    public event EventHandler<string> OnLogMessagePublished;

    public TwitchConnector(GameSettings gameSettings, 
        List<IHandleChatCommand> chatCommandHandlers)
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
        _chatCommandHandlers = chatCommandHandlers;

        var botAccountName = 
            string.IsNullOrWhiteSpace(gameSettings.BotAccountName)
                ? gameSettings.ChannelName
                : gameSettings.BotAccountName;

        _credentials =
            new ConnectionCredentials(
                botAccountName, 
                gameSettings.TwitchToken,
                disableUsernameCheck: true);

        SubscribeToEvents();
    }

    public void Connect()
    {
        try
        {
            _client.Initialize(_credentials, _channelName);

            if (_hasConnected)
            {
                _client.Reconnect();
            }
            else
            {
                _client.Connect();
                _hasConnected = true;
            }
        }
        catch (Exception e)
        {
            OnLogMessagePublished(this, $"Exception during Connect(): {e.Message}");
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

    private void SubscribeToEvents()
    {
        _client.OnConnected += HandleConnected;
        _client.OnDisconnected += HandleDisconnected;
        _client.OnChatCommandReceived += HandleChatCommandReceived;
    }

    private void UnsubscribeFromEvents()
    {
        _client.OnConnected -= HandleConnected;
        _client.OnDisconnected -= HandleDisconnected;
        _client.OnChatCommandReceived -= HandleChatCommandReceived;
    }

    private void HandleConnected(object? sender, OnConnectedArgs e)
    {
        OnConnected?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDisconnected(object? sender, OnDisconnectedEventArgs e)
    {
        OnDisconnected?.Invoke(this, EventArgs.Empty);

        Connect();
    }

    private void HandleChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        OnLogMessagePublished
            .Invoke(this, 
                $"{e.Command.ChatMessage.DisplayName} - {e.Command.ChatMessage.Message}");

        _chatCommandHandlers
            .FirstOrDefault(cch => cch.CommandName.Matches(e.Command.CommandText))
            ?.Execute(e.Command);
    }
}