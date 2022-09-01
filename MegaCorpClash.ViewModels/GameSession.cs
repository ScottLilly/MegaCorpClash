using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Services;

namespace MegaCorpClash.ViewModels;

public class GameSession
{
    private readonly LogWriter _logWriter = new();
    private readonly GameSettings _gameSettings;
    private readonly Dictionary<string, Player> _players = new();
    private readonly TwitchConnector? _twitchConnector;
    private readonly PointsCalculator _pointsCalculator;

    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private System.Timers.Timer _pointsTimer;

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        PopulatePlayers();

        _pointsCalculator = new PointsCalculator(gameSettings, _players);

        var chatCommandHandlers = GetChatCommandHandlers();

        _twitchConnector = new TwitchConnector(gameSettings, chatCommandHandlers);
        _twitchConnector.OnConnected += HandleConnected;
        _twitchConnector.OnDisconnected += HandleDisconnected;
        _twitchConnector.OnPersonChatted += HandlePersonChatted;
        _twitchConnector.OnLogMessagePublished += HandleLogMessagePublished;
        _twitchConnector.Connect();

        InitializePointsTimer();
        InitializeTimedMessages(gameSettings.TimedMessages);
    }

    public List<string> ShowPlayers()
    {
        return _players
            .OrderBy(p => p.Value.DisplayName)
            .Select(p => $"[{p.Value.DisplayName}] {p.Value.CompanyName} - {p.Value.Points} Employee count: [{p.Value.Employees.Count}]")
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

        foreach (Player player in players)
        {
            _players.Add(player.Id, player);
        }
    }

    private List<BaseCommandHandler> GetChatCommandHandlers()
    {
        var baseType = typeof(BaseCommandHandler);
        var assembly = baseType.Assembly;

        List<BaseCommandHandler> commandHandlers = 
            assembly.GetTypes()
            .Where(t => t.IsSubclassOf(baseType))
            .Select(t => Activator.CreateInstance(t, new object[] {_gameSettings, _players}))
            .Cast<BaseCommandHandler>()
            .ToList();

        foreach(BaseCommandHandler commandHandler in commandHandlers)
        {
            commandHandler.OnChatMessagePublished += HandleChatMessagePublished;
            commandHandler.OnPlayerDataUpdated += HandlePlayerDataUpdated;
        }

        return commandHandlers;
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
        WriteMessageToLog(e);
    }

    private void HandleChatMessagePublished(object? sender, ChatMessageEventArgs e)
    {
        WriteMessageToTwitchChat($"{e.ChatterDisplayName} {e.Message}");
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
        WriteMessageToLog($"Points timer ticked: {DateTime.Now}");

        _pointsCalculator.ApplyPointsForTurn();

        UpdatePlayerInformation();
    }

    #endregion

    #region Private output/display functions

    private void WriteMessageToTwitchChat(string message)
    {
        _twitchConnector?.SendChatMessage(message);

        WriteMessageToLog(message);
    }

    private void WriteMessageToLog(string message)
    {
        Console.WriteLine(message);
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