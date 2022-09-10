using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.ChatConnectors;
using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Services;

namespace MegaCorpClash.ViewModels;

public class GameSession
{
    private readonly LogWriter _logWriter = new();
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Company> _players = new();
    private readonly IChatConnector? _twitchConnector;
    private readonly PointsCalculator _pointsCalculator;

    private List<BaseCommandHandler> _gameCommandHandlers = new();
    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private System.Timers.Timer _pointsTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        PopulatePlayers();
        PopulateGameCommandHandlers();

        _pointsCalculator = new PointsCalculator(gameSettings, _players);

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

    public List<string> ShowPlayers()
    {
        return _players
            .OrderBy(p => p.Value.ChatterName)
            .Select(p => $"[{p.Value.ChatterName}] {p.Value.CompanyName} - {p.Value.Points} Employee count: [{p.Value.Employees.Count}]")
            .ToList();
    }

    public void End()
    {
        WriteMessageToTwitchChat("Stopping MegaCorpClash game. Thanks for playing!");

        _twitchConnector?.Disconnect();
    }

    #region Private startup functions

    private void PopulatePlayers()
    {
        var players = PersistenceService.GetPlayerData();

        foreach (Company player in players)
        {
            player.IsBroadcaster = 
                player.ChatterName.Matches(_gameSettings.ChannelName);

            _players.Add(player.ChatterId, player);
        }
    }

    private void PopulateGameCommandHandlers()
    {
        var baseType = typeof(BaseCommandHandler);
        var assembly = baseType.Assembly;

        _gameCommandHandlers = 
            assembly.GetTypes()
            .Where(t => t.IsSubclassOf(baseType))
            .Select(t => Activator.CreateInstance(t, _gameSettings, _players))
            .Cast<BaseCommandHandler>()
            .ToList();

        foreach(BaseCommandHandler commandHandler in _gameCommandHandlers)
        {
            commandHandler.OnChatMessagePublished += HandleChatMessagePublished;
            commandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
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
        _pointsCalculator.RecordPlayerChatted(e.UserId);
    }

    private void HandleLogMessagePublished(object? sender, string e)
    {
        WriteMessageToLogFile(e);
    }

    private void HandleChatMessagePublished(object? sender, ChatMessageEventArgs e)
    {
        WriteMessageToTwitchChat($"{e.ChatterDisplayName} {e.Message}");
    }

    private void HandleGameCommandReceived(object? sender, GameCommand e)
    {
        BaseCommandHandler? gameCommandHandler =
            _gameCommandHandlers
                .FirstOrDefault(cch => cch.CommandName.Matches(e.CommandName));

        if (gameCommandHandler == null)
        {
            return;
        }

        WriteMessageToLogFile($"[{e.ChatterName}] {e.CommandName} {e.Argument}");

        gameCommandHandler?.Execute(e);
    }

    private void HandlePlayerDataUpdated(object? sender, EventArgs e)
    {
        UpdatePlayerInformation();
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
        _pointsTimer =
            new System.Timers.Timer(_gameSettings.TurnDetails.MinutesPerTurn * 60 * 1000);
        _pointsTimer.Elapsed += PointsTimer_Elapsed;
        _pointsTimer.Enabled = true;
    }

    private void TimedMessagesTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        WriteMessageToTwitchChat(_timedMessages?.RandomElement() ?? "");
    }

    private void PointsTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        string message = $"Points timer ticked: {DateTime.Now}";

        WriteToConsole(message);
        WriteMessageToLogFile(message);

        _pointsCalculator.ApplyPointsForTurn();

        UpdatePlayerInformation();
    }

    #endregion

    #region Private output/display functions

    private void WriteMessageToTwitchChat(string message)
    {
        _twitchConnector?.SendChatMessage(message);

        WriteMessageToLogFile(message);
    }

    private void WriteToConsole(string message)
    {
        Console.WriteLine(message);
    }

    private void WriteMessageToLogFile(string message)
    {
        _logWriter.WriteMessage(message);
    }

    #endregion

    #region Private support functions

    private void UpdatePlayerInformation()
    {
        PersistenceService.SavePlayerData(_players.Values);
    }

    #endregion
}