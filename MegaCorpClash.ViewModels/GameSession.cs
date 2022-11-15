﻿using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services;
using MegaCorpClash.Services.ChatConnectors;
using MegaCorpClash.Services.CustomEventArgs;
using MegaCorpClash.Services.Queues;

namespace MegaCorpClash.ViewModels;

public sealed class GameSession
{
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _companies = new();
    private readonly IChatConnector? _twitchConnector;
    private readonly PointsCalculator _pointsCalculator;
    private readonly CommandHandlerFactory _commandHandlerFactory;
    private readonly CommandHandlerQueue _commandHandlerQueueManager;

    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private System.Timers.Timer _pointsCalculatorTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        _commandHandlerQueueManager = 
            new CommandHandlerQueue(_gameSettings.MinimumSecondsBetweenCommands);

        PopulateCompanies();

        _commandHandlerFactory = 
            new CommandHandlerFactory(_gameSettings, _companies);

        _commandHandlerQueueManager.OnChatMessagePublished += HandleChatMessageToSend;
        _commandHandlerQueueManager.OnCompanyDataUpdated += HandleCompanyDataUpdated;
        _commandHandlerQueueManager.OnBankruptedStreamer += HandleBankruptedStreamer;
        _commandHandlerQueueManager.OnLogMessagePublished += HandleLogMessagePublished;

        _pointsCalculator = new PointsCalculator(gameSettings, _companies);

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
        _commandHandlerQueueManager.Stop();

        _twitchConnector?.Disconnect();
    }

    public void AddBonusPointsNextTurn(int bonusPoints)
    {
        _pointsCalculator.SetBonusPointsForNextTurn(bonusPoints);
    }

    public void SetStreamMultiplier(int multiplier)
    {
        _pointsCalculator.SetStreamMultiplier(multiplier);
    }

    #endregion

    #region Private startup functions

    private void PopulateCompanies()
    {
        var companies = PersistenceService.GetPlayerData();

        foreach (Company company in companies)
        {
            company.IsBroadcaster = 
                company.DisplayName.Matches(_gameSettings.TwitchBroadcasterAccount?.Name ?? "");

            _companies.Add(company.UserId, company);
        }
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

        _pointsCalculator.RecordPlayerChatted(e.UserId);
    }

    private void HandleGameCommandReceived(object? sender, GameCommandArgs e)
    {
        var command =
            _commandHandlerFactory.GetCommandHandlerForCommand(e.CommandName);

        if (command == null ||
            (e.IsBroadcaster && !command.BroadcasterCanRun))
        {
            return;
        }

        _commandHandlerQueueManager.Add((command, e));

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

    private void HandleCompanyDataUpdated(object? sender, EventArgs e)
    {
        UpdatePlayerInformation();
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

        _pointsCalculator.ApplyPointsForTurn();

        UpdatePlayerInformation();
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

    private void UpdatePlayerInformation()
    {
        //PersistenceService.SavePlayerData(_companies.Values);
    }

    private void UpdateChatterDetailsIfChanged(ChattedEventArgs eventArgs)
    {
        if (_companies.TryGetValue(eventArgs.UserId, out Company? company) &&
            (company.DisplayName != eventArgs.DisplayName ||
             company.IsSubscriber != eventArgs.IsSubscriber ||
             company.IsVip != eventArgs.IsVip))
        {
            company.DisplayName = eventArgs.DisplayName;
            company.IsSubscriber = eventArgs.IsSubscriber;
            company.IsVip = eventArgs.IsVip;

            UpdatePlayerInformation();
        }
    }

    #endregion
}