using MegaCorpClash.Models;
using MegaCorpClash.Services.BroadcasterCommandHandlers;
using MegaCorpClash.Services.ChatCommandHandlers;
using MegaCorpClash.Services.CustomEventArgs;
using System.Collections.Concurrent;

namespace MegaCorpClash.Services.Queues;

public class GameEventQueue :
    BaseTypedQueue<IExecutable>
{
    private readonly int _minimumSecondsBetweenCommands;
    private readonly ConcurrentDictionary<string, CommandTimestamp> _lastCommandTimestamp = new();
    public event EventHandler<BankruptedStreamerArgs> OnBankruptedStreamer;
    public event EventHandler<BroadcasterCommandEventArgs> OnBroadcasterCommand;

    public GameEventQueue(int minimumSecondsBetweenCommands)
    {
        _minimumSecondsBetweenCommands = minimumSecondsBetweenCommands;

        Task.Factory.StartNew(Consumer);
    }

    private void Consumer()
    {
        foreach (var commandToExecute in _queue.GetConsumingEnumerable())
        {
            if(commandToExecute is BroadcasterOnlyCommandHandler broadcasterCommand)
            {
                broadcasterCommand.OnBroadcasterCommand += HandleBroadcasterCommand;
                broadcasterCommand.Execute();
                broadcasterCommand.OnBroadcasterCommand -= HandleBroadcasterCommand;
                continue;
            }

            if (commandToExecute is not BaseCommandHandler)
            {
                commandToExecute.Execute();
                continue;
            }

            var commandHandler = (BaseCommandHandler)commandToExecute;

            var chatterDetails = commandHandler.ChatterDetails();

            _lastCommandTimestamp.TryGetValue(
                chatterDetails.ChatterId,
                out var chattersLastCommand);

            if (ChatIsThrottled() &&
                (!chatterDetails.Company?.IsBroadcaster ?? true) &&
                chattersLastCommand != null)
            {
                if ((DateTime.UtcNow -
                     chattersLastCommand.TimeChatted).TotalSeconds <
                     _minimumSecondsBetweenCommands)
                {
                    WarnChatterOfThrottlingCondition(chatterDetails);

                    continue;
                }
            }

            _lastCommandTimestamp[chatterDetails.ChatterId] =
                new CommandTimestamp(DateTime.UtcNow, false);

            // TODO: Get back command arguments
            PublishLogMessage($"[{chatterDetails.ChatterName}] {commandHandler.CommandName}");

            commandToExecute.Execute();

            foreach (var message in commandHandler.ChatMessages)
            {
                PublishChatMessage(chatterDetails.ChatterName, message);
            }

            NotifyIfStreamerBankrupted(commandHandler);
        }
    }

    private void HandleBroadcasterCommand(object? sender, BroadcasterCommandEventArgs e)
    {
        OnBroadcasterCommand.Invoke(sender, e);
    }

    private void WarnChatterOfThrottlingCondition(
        ChatterDetails chatterDetails)
    {
        if (_lastCommandTimestamp[chatterDetails.ChatterId].HasBeenWarned)
        {
            return;
        }

        PublishChatMessage(
            chatterDetails.ChatterName,
            $"Please wait {_minimumSecondsBetweenCommands} seconds between commands");

        _lastCommandTimestamp[chatterDetails.ChatterId].HasBeenWarned = true;
    }

    private void NotifyIfStreamerBankrupted(BaseCommandHandler commandHandler)
    {
        if (commandHandler.StreamerBankrupted)
        {
            OnBankruptedStreamer?.Invoke(this,
                new BankruptedStreamerArgs());
        }
    }

    private bool ChatIsThrottled() =>
        _minimumSecondsBetweenCommands > 0;

    private class CommandTimestamp
    {
        public DateTime TimeChatted { get; set; }
        public bool HasBeenWarned { get; set; }

        public CommandTimestamp(DateTime timeChatted, bool hasBeenWarned)
        {
            TimeChatted = timeChatted;
            HasBeenWarned = hasBeenWarned;
        }
    }

}