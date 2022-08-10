using System.Timers;
using MegaCorpClash.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace MegaCorpClash.ViewModels;

public class GameSession : IDisposable
{
    private const string CHAT_LOG_DIRECTORY = "./ChatLogs";

    private readonly GameSettings _gameSettings;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchClient _client = new();

    private System.Timers.Timer _timedMessagesTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        _credentials =
            new ConnectionCredentials(
                string.IsNullOrWhiteSpace(_gameSettings.BotAccountName)
                    ? _gameSettings.ChannelName
                    : _gameSettings.BotAccountName, _gameSettings.TwitchToken, 
                disableUsernameCheck: true);

        _client.OnMessageReceived += HandleChatMessageReceived;
        _client.OnChatCommandReceived += HandleChatCommandReceived;
        _client.OnDisconnected += HandleDisconnected;

        InitializeTimedMessages();

        ConnectToTwitch();
    }

    public void Dispose()
    {
        _client.OnChatCommandReceived -= HandleChatCommandReceived;
        _client.OnDisconnected -= HandleDisconnected;

        _client.Disconnect();
    }

    private void ConnectToTwitch()
    {
        _client.Initialize(_credentials, _gameSettings.ChannelName);
        _client.Connect();
    }

    private void InitializeTimedMessages()
    {
        if (!(_gameSettings.HelpMessageIntervalInMinutes > 0))
        {
            return;
        }

        _timedMessagesTimer =
            new System.Timers.Timer(_gameSettings.HelpMessageIntervalInMinutes * 60 * 1000);
        _timedMessagesTimer.Elapsed += TimedMessagesTimer_Elapsed;
        _timedMessagesTimer.Enabled = true;
    }

    private void HandleChatMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        WriteToChatLog(e.ChatMessage.DisplayName, e.ChatMessage.Message);
    }

    private void HandleChatCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        WriteToChatLog(e.Command.ChatMessage.DisplayName, e.Command.ChatMessage.Message);
    }

    private void HandleDisconnected(object? sender, OnDisconnectedEventArgs e)
    {
        // If disconnected, automatically attempt to reconnect
        ConnectToTwitch();
    }

    private void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _client.SendMessage(_gameSettings.ChannelName, message);

        WriteToChatLog(_gameSettings.BotDisplayName, message);
    }

    private void TimedMessagesTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        SendChatMessage("Test");
    }

    private void WriteToChatLog(string chatterName, string message)
    {
        if (!Directory.Exists(CHAT_LOG_DIRECTORY))
        {
            Directory.CreateDirectory(CHAT_LOG_DIRECTORY);
        }

        File.AppendAllText(
            Path.Combine(CHAT_LOG_DIRECTORY, $"MegaCorpClash-{DateTime.Now:yyyy-MM-dd}.log"),
            $"{DateTime.Now.ToShortTimeString()}: {chatterName} - {message}{Environment.NewLine}");
    }
}