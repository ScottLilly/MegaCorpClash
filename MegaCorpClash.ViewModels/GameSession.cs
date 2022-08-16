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

    private static void OnCompanyCreated(object? sender, CompanyCreatedArgs e)
    {
        // TODO: If Player or company name already exists, send an error message.
        // Else, add Player and company to players list and write to disk.
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