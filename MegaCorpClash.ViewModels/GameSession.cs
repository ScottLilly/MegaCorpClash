using System.Timers;
using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;
using MegaCorpClash.Services;

namespace MegaCorpClash.ViewModels;

public class GameSession : IDisposable
{
    private readonly GameSettings _gameSettings;
    private TwitchConnector? _twitchConnector;
    private List<string> _timedMessages = new();
    private System.Timers.Timer? _timedMessagesTimer;
    private readonly LogWriter _logWriter = new();

    private readonly Dictionary<string, Player> _players = new();

    public GameSession(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;

        PopulatePlayers();

        var chatCommandHandlers = GetChatCommandHandlers();

        _twitchConnector = new TwitchConnector(gameSettings, chatCommandHandlers);
        _twitchConnector.OnMessageToLog += OnTwitchMessageToLog;
        _twitchConnector.Connect();

        Thread.Sleep(5000);

        InitializeTimedMessages(gameSettings.TimedMessages);

        LogMessage("Starting MegaCorpClash game", true);
    }

    private List<IHandleChatCommand> GetChatCommandHandlers()
    {
        var chatCommandHandlers = 
            new List<IHandleChatCommand>();

        var incorporateCommandHandler = new IncorporateCommandHandler(_players);
        incorporateCommandHandler.OnMessageToDisplay += OnMessageToDisplay;
        incorporateCommandHandler.OnPlayerDataUpdated += OnPlayerDataUpdated;
        chatCommandHandlers.Add(incorporateCommandHandler);

        var renameCommandHandler = new RenameCommandHandler(_players);
        renameCommandHandler.OnMessageToDisplay += OnMessageToDisplay;
        renameCommandHandler.OnPlayerDataUpdated += OnPlayerDataUpdated;
        chatCommandHandlers.Add(renameCommandHandler);

        var statusCommandHandler = new StatusCommandHandler(_players, _gameSettings.PointsName);
        statusCommandHandler.OnMessageToDisplay += OnMessageToDisplay;
        chatCommandHandlers.Add(statusCommandHandler);

        var companiesCommandHandler = new CompaniesCommandHandler(_players);
        companiesCommandHandler.OnMessageToDisplay += OnMessageToDisplay;
        chatCommandHandlers.Add(companiesCommandHandler);

        return chatCommandHandlers;
    }

    private void OnPlayerDataUpdated(object? sender, EventArgs e)
    {
        PersistenceService.SavePlayerData(_players.Values);
    }

    private void OnMessageToDisplay(object? sender, MessageEventArgs e)
    {
        LogMessage(e.Message, e.ShowInTwitchChat);
    }

    public void Dispose()
    {
        LogMessage("Stopping MegaCorpClash game. Thanks for playing!", true);

        _twitchConnector?.Dispose();
        _twitchConnector = null;
    }

    private void PopulatePlayers()
    {
        var players = PersistenceService.GetPlayerData();

        foreach (Player player in players)
        {
            _players.Add(player.Id, player);
        }
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
        SendMessageInTwitchChat(_timedMessages?.RandomElement() ?? "");
    }

    private void OnTwitchMessageToLog(object? sender, string e)
    {
        WriteToLog(e);
    }

    private void SendMessageInTwitchChat(string message)
    {
        LogMessage(message, true);
    }

    private void LogMessage(string message, bool writeInTwitchChat)
    {
        if (writeInTwitchChat)
        {
            _twitchConnector?.SendChatMessage(message);
        }

        WriteToLog(message);
    }

    private void WriteToLog(string message)
    {
        Console.WriteLine(message);
        _logWriter.WriteMessage(message);
    }
}