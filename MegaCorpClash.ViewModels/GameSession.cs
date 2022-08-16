using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Services;

namespace MegaCorpClash.ViewModels;

public class GameSession : IDisposable
{
    private TwitchConnector? _twitchConnector;
    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private readonly LogWriter _logWriter = new();

    private Dictionary<string, Player> _players = new();

    public GameSession(GameSettings gameSettings)
    {
        _twitchConnector = new TwitchConnector(gameSettings);
        _twitchConnector.OnMessageToLog += OnTwitchMessageToLog;
        _twitchConnector.OnCompanyCreated += OnCompanyCreated;
        _twitchConnector.Connect();

        InitializeTimedMessages(gameSettings.TimedMessages);

        WriteToLog("Game started");
    }

    public void Dispose()
    {
        _twitchConnector?.Dispose();
        _twitchConnector = null;
    }

    private void InitializeTimedMessages(
        GameSettings.TimedMessageSettings timedMessageSettings)
    {
        if (timedMessageSettings.IntervalInMinutes <= 0)
        {
            return;
        }

        _timedMessages = timedMessageSettings.Messages;

        _timedMessagesTimer =
            new System.Timers.Timer(timedMessageSettings.IntervalInMinutes * 60 * 1000);
        _timedMessagesTimer.Elapsed += TimedMessagesTimer_Elapsed;
        _timedMessagesTimer.Enabled = true;
    }

    private void OnCompanyCreated(object? sender, CompanyCreatedArgs e)
    {
        _players.TryGetValue(e.TwitchId, out Player? player);

        if (player == null)
        {
            player = new Player
            {
                Id = e.TwitchId,
                DisplayName = e.TwitchDisplayName,
                CompanyName = e.CompanyName,
                CreatedOn = DateTime.UtcNow
            };

            _players[e.TwitchId] = player;
        }
        else
        {
            _twitchConnector?
                .SendChatMessage($"{e.TwitchDisplayName}, you already have a company name {player.CompanyName}");
        }
    }

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _twitchConnector?
            .SendChatMessage(_timedMessages.RandomElement());
    }

    private void OnTwitchMessageToLog(object? sender, string e)
    {
        WriteToLog(e);
    }

    private void WriteToLog(string message)
    {
        Console.WriteLine(message);
        _logWriter.WriteMessage(message);
    }
}