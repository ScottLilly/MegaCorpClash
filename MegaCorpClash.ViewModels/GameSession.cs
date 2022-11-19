using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services;
using MegaCorpClash.Services.ChatConnectors;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Persistence;
using MegaCorpClash.Services.Queues;

namespace MegaCorpClash.ViewModels;

public sealed class GameSession
{
    private IRepository _companyRepository =
        CompanyRepository.GetInstance();

    private readonly GameSettings _gameSettings;
    private readonly IChatConnector? _twitchConnector;
    private readonly PointsCalculatorFactory _pointsCalculatorFactory;
    private readonly CommandHandlerFactory _commandHandlerFactory;
    private readonly GameEventQueue _gameEventQueue;

    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private System.Timers.Timer _pointsCalculatorTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        _gameEventQueue = 
            new GameEventQueue(_gameSettings.MinimumSecondsBetweenCommands);

        _commandHandlerFactory = 
            new CommandHandlerFactory(_gameSettings, _companyRepository);

        _pointsCalculatorFactory = 
            new PointsCalculatorFactory(gameSettings.TurnDetails.PointsPerTurn, _companyRepository);

        _gameEventQueue.OnChatMessagePublished += HandleChatMessageToSend;
        _gameEventQueue.OnBankruptedStreamer += HandleBankruptedStreamer;
        _gameEventQueue.OnLogMessagePublished += HandleLogMessagePublished;

        _twitchConnector = new TwitchConnector(gameSettings);
        _twitchConnector.OnConnected += HandleConnected;
        _twitchConnector.OnDisconnected += HandleDisconnected;
        _twitchConnector.OnPersonChatted += HandlePersonChatted;
        _twitchConnector.OnGameCommandReceived += HandleGameCommandReceived;
        _twitchConnector.OnLogMessagePublished += HandleLogMessagePublished;
        _twitchConnector.Connect();

        InitializePointsTimer();
        InitializeTimedMessages(gameSettings.TimedMessages);
    }
    
    #region Public functions

    public void End()
    {
        WriteMessageToTwitchChat("Stopping MegaCorpClash game. Thanks for playing!");

        _timedMessagesTimer?.Stop();
        _pointsCalculatorTimer?.Stop();
        _gameEventQueue.Stop();

        _twitchConnector?.Disconnect();
    }

    public void AddBonusPointsNextTurn(int bonusPoints)
    {
        _pointsCalculatorFactory.SetBonusPointsForNextTurn(bonusPoints);
    }

    public void SetStreamMultiplier(int multiplier)
    {
        _pointsCalculatorFactory.SetStreamMultiplier(multiplier);
    }

    #endregion

    #region Private event handler functions

    private void HandleConnected(object? sender, EventArgs e)
    {
        WriteMessageToTwitchChat("Started MegaCorpClash game");
    }

    private void HandleDisconnected(object? sender, EventArgs e)
    {
        WriteMessageToTwitchChat("Stopped MegaCorpClash game");
    }

    private void HandlePersonChatted(object? sender, ChattedEventArgs e)
    {
        UpdateChatterDetailsIfChanged(e);

        _pointsCalculatorFactory.RecordPlayerChatted(e.UserId);
    }

    private void HandleGameCommandReceived(object? sender, GameCommandArgs e)
    {
        var command =
            _commandHandlerFactory.GetCommandHandlerForCommand(e);

        if (command == null ||
            (e.IsBroadcaster && !command.BroadcasterCanRun))
        {
            return;
        }

        _gameEventQueue.Add(command);

        UpdateChatterDetailsIfChanged(e);
    }

    private void HandleLogMessagePublished(object? sender, LogMessageEventArgs e)
    {
        WriteMessageToLogFile(e.Message);
    }

    private void HandleChatMessageToSend(object? sender, ChatMessageEventArgs e)
    {
        WriteMessageToTwitchChat($"{e.DisplayName} {e.Message}");
    }

    private void HandleBankruptedStreamer(object? sender, BankruptedStreamerArgs e)
    {
        WriteMessageToLogFile("Bankrupted streamer");
        WriteMessageToTwitchChat("Bankrupted streamer");
    }

    #endregion

    #region Private timer function methods

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

    private void InitializePointsTimer()
    {
        _pointsCalculatorTimer =
            new System.Timers.Timer(_gameSettings.TurnDetails.MinutesPerTurn * 60 * 1000);
        _pointsCalculatorTimer.Elapsed += PointsCalculatorTimerElapsed;
        _pointsCalculatorTimer.Enabled = true;
    }

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        string? message = _timedMessages?.RandomElement();

        if(message == null ||
            string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if(message.StartsWith("!"))
        {
            HandleGameCommandReceived(this,
                new GameCommandArgs(
                    "", "", message.Substring(1), "", false, false, false, false));
        }
        else 
        {
            WriteMessageToTwitchChat(message ?? "");
        }
    }

    private void PointsCalculatorTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        string message = $"Points timer ticked: {DateTime.Now}";

        WriteToConsole(message);
        WriteMessageToLogFile(message);

        _gameEventQueue.Add(_pointsCalculatorFactory.GetPointsCalculator());
    }

    private void StopTimers()
    {
        _timedMessagesTimer?.Stop();
        _pointsCalculatorTimer?.Stop();
    }

    #endregion

    #region Private output/display functions

    private void WriteMessageToTwitchChat(string message)
    {
        _twitchConnector?.SendChatMessage(message);

        WriteMessageToLogFile(message);
    }

    private static void WriteToConsole(string message)
    {
        Console.WriteLine(message);
    }

    private static void WriteMessageToLogFile(string message)
    {
        LogWriter.WriteMessage($"{DateTime.UtcNow:u} {message}");
    }

    #endregion

    #region Private support functions

    private void UpdateChatterDetailsIfChanged(ChattedEventArgs eventArgs)
    {
        var company = _companyRepository.GetCompany(eventArgs.UserId);

        if (company != null &&
            (company.DisplayName != eventArgs.DisplayName ||
             company.IsSubscriber != eventArgs.IsSubscriber ||
             company.IsVip != eventArgs.IsVip))
        {
            company.DisplayName = eventArgs.DisplayName;
            company.IsSubscriber = eventArgs.IsSubscriber;
            company.IsVip = eventArgs.IsVip;

            _companyRepository.UpdateCompany(company);
        }
    }

    #endregion
}