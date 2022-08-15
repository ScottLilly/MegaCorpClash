using System.Timers;
using MegaCorpClash.Core;
using MegaCorpClash.Models;

namespace MegaCorpClash.ViewModels;

public class GameSession : IDisposable
{
    private readonly GameSettings _gameSettings;
    private TwitchConnector? _twitchConnector;
    private System.Timers.Timer? _timedMessagesTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
        _twitchConnector = new TwitchConnector(gameSettings);

        InitializeTimedMessages(_gameSettings.TimedMessages.IntervalInMinutes);

        _twitchConnector.Connect();
    }

    public void Dispose()
    {
        _twitchConnector?.Dispose();
        _twitchConnector = null;
    }

    private void InitializeTimedMessages(int intervalInMinutes)
    {
        if (!(intervalInMinutes > 0))
        {
            return;
        }

        _timedMessagesTimer =
            new System.Timers.Timer(intervalInMinutes * 60 * 1000);
        _timedMessagesTimer.Elapsed += TimedMessagesTimer_Elapsed;
        _timedMessagesTimer.Enabled = true;
    }

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (_gameSettings.TimedMessages.Messages.Any())
        {
            _twitchConnector?
                .SendChatMessage(_gameSettings.TimedMessages.Messages.RandomElement());
        }
    }
}