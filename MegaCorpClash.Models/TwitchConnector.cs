using CSharpExtender.ExtensionMethods;
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

    public event EventHandler<CompanyCreatedEventArgs>? OnCompanyCreated;
    public event EventHandler<CompanyNameChangedEventArgs> OnCompanyNameChanged;
    public event EventHandler<string> OnMessageToLog;

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
        _client.Connect();
        WriteToChatLog("Connected to Twitch");
    }

    public void SendChatMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _client.SendMessage(_channelName, message);
        Console.WriteLine(message);
        WriteToChatLog(_botAccountName, message);
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
        WriteToChatLog(e.Command.ChatMessage.DisplayName, e.Command.ChatMessage.Message);

        if (e.Command.CommandText.Matches("incorporate"))
        {
            if (e.Command.ArgumentsAsList.Any())
            {
                OnCompanyCreated?.Invoke(this, new CompanyCreatedEventArgs(e.Command));
            }
            else
            {
                SendChatMessage($"{e.Command.ChatMessage.DisplayName} - !incorporate must be followed by name for your company");
            }
        }
        else if (e.Command.CommandText.Matches("rename"))
        {
            if (e.Command.ArgumentsAsList.Any())
            {
                OnCompanyNameChanged?.Invoke(this, new CompanyNameChangedEventArgs(e.Command));
            }
            else
            {
                SendChatMessage($"{e.Command.ChatMessage.DisplayName} - !rename must be followed by the new name for your company");
            }
        }
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