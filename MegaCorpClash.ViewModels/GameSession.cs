using System.Timers;
using MegaCorpClash.Core;
using MegaCorpClash.Models;

namespace MegaCorpClash.ViewModels;

public class GameSession : IDisposable
{
    private TwitchConnector? _twitchConnector;
    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;

    public GameSession(GameSettings gameSettings)
    {
        _twitchConnector = new TwitchConnector(gameSettings);
        _twitchConnector.Connect();

        InitializeTimedMessages(gameSettings.TimedMessages);
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

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _twitchConnector?
            .SendChatMessage(_timedMessages.RandomElement());
    }
}