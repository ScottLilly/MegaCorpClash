using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models.CustomEventArgs;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace MegaCorpClash.Models;

public class TwitchConnector : IDisposable
{
    private const string CHAT_LOG_DIRECTORY = "./ChatLogs";

    private readonly string _channelName;
    private readonly string _botDisplayName;
    private readonly ConnectionCredentials _credentials;
    private readonly TwitchClient _client = new();

    public event EventHandler<CompanyCreatedArgs>? OnCompanyCreated;

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

        var botAccountName = 
            string.IsNullOrWhiteSpace(gameSettings.BotAccountName)
                ? gameSettings.ChannelName
                : gameSettings.BotAccountName;

        _botDisplayName = 
            string.IsNullOrWhiteSpace(gameSettings.BotDisplayName) 
                ? gameSettings.BotAccountName
                : gameSettings.BotDisplayName;

        _credentials =
            new ConnectionCredentials(
                botAccountName, 
                gameSettings.TwitchToken,
                disableUsernameCheck: true);

        SubscribeToEvents();
    }

    public void Connect()
    {
        _client.Initialize(_credentials, _channelName);
        _client.Connect();
    }

    public void SendChatMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        _client.SendMessage(_channelName, message);
        Console.WriteLine(message);
        WriteToChatLog(_botDisplayName, message);
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
                OnCompanyCreated?.Invoke(this, new CompanyCreatedArgs(e.Command));
            }
            else
            {
                SendChatMessage($"{e.Command.ChatMessage.DisplayName} - !incorporate must be followed by single word/name for your company");
            }
        }
    }

    private void HandleDisconnected(object? sender, OnDisconnectedEventArgs e)
    {
        // If disconnected, automatically attempt to reconnect
        Connect();
    }

    private static void WriteToChatLog(string chatterName, string? message)
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