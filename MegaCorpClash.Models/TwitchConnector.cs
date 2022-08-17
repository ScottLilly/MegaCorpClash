using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace MegaCorpClash.Models;

public class TwitchConnector : IDisposable
{
    private readonly string _channelName;
    private readonly string _botAccountName;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchClient _client = new();

    public event EventHandler<string> OnMessageToLog;

    private readonly List<IHandleChatCommand> _chatCommandHandlers;

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

        _chatCommandHandlers = chatCommandHandlers;
        _channelName = gameSettings.ChannelName;

        _botAccountName = 
            string.IsNullOrWhiteSpace(gameSettings.BotAccountName)
                ? gameSettings.ChannelName
                : gameSettings.BotAccountName;

        _credentials =
            new ConnectionCredentials(
                _botAccountName, 
                gameSettings.TwitchToken,
                disableUsernameCheck: true);

        SubscribeToEvents();
    }

    public void Connect()
    {
        WriteToChatLog("Start connecting to Twitch");
        _client.Initialize(_credentials, _channelName);
        try
        {
            _client.Connect();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        WriteToChatLog("Connected to Twitch");
    }

    public void SendChatMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _client.SendMessage(_channelName, message);
    }

    public void Dispose()
    {
        UnsubscribeFromEvents();

        _client?.Disconnect();
    }

    private void SubscribeToEvents()
    {
        _client.OnChatCommandReceived += HandleChatCommandReceived;
        _client.OnDisconnected += HandleDisconnected;
    }

    private void UnsubscribeFromEvents()
    {
        _client.OnChatCommandReceived -= HandleChatCommandReceived;
        _client.OnDisconnected -= HandleDisconnected;
    }

    private void HandleChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        OnMessageToLog?
            .Invoke(this, 
                $"{e.Command.ChatMessage.DisplayName} - {e.Command.ChatMessage.Message}");

        _chatCommandHandlers
            .FirstOrDefault(cch => cch.CommandText.Matches(e.Command.CommandText))
            ?.Execute(e.Command);
    }

    private void HandleDisconnected(object? sender, OnDisconnectedEventArgs e)
    {
        // If disconnected, automatically attempt to reconnect
        WriteToChatLog("Disconnected - Reconnecting");
        Connect();
    }

    private void WriteToChatLog(string chatterName, string? message)
    {
        WriteToChatLog($"{chatterName} - {message}");
    }

    private void WriteToChatLog(string? message)
    {
        OnMessageToLog?.Invoke(this,
            $"{DateTime.Now.ToShortTimeString()}: {message}");
    }
}