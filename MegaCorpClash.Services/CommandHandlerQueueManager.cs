using System.Collections.Concurrent;
using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Services;

public class CommandHandlerQueueManager : 
    BaseQueueManager<(BaseCommandHandler, GameCommandArgs)>
{
    private readonly int _minimumSecondsBetweenCommands;
    private readonly ConcurrentDictionary<string, DateTime> _lastCommandRun = new();

    public event EventHandler<ChatMessageEventArgs> OnChatMessageToSend;
    public event EventHandler OnPlayerDataUpdated;
    public event EventHandler<BankruptedStreamerArgs> OnBankruptedStreamer;

    public CommandHandlerQueueManager(int minimumSecondsBetweenCommands)
    {
        _minimumSecondsBetweenCommands = minimumSecondsBetweenCommands;

        Task.Factory.StartNew(Consumer);
    }

    private void Consumer()
    {
        foreach (var item in _queue.GetConsumingEnumerable())
        {
            var commandHandler = item.Item1;
            var commandArgs = item.Item2;
            var chatterDetails = commandHandler.ChatterDetails(commandArgs);

            if (_minimumSecondsBetweenCommands > 0 &&
                _lastCommandRun.ContainsKey(chatterDetails.ChatterId))
            {
                if ((DateTime.UtcNow - 
                     _lastCommandRun[chatterDetails.ChatterId]).TotalSeconds < _minimumSecondsBetweenCommands)
                {
                    OnChatMessageToSend?.Invoke(this,
                        new ChatMessageEventArgs(chatterDetails.ChatterName, 
                            $"Please wait {_minimumSecondsBetweenCommands} seconds between commands"));

                    continue;
                }
            }

            _lastCommandRun[chatterDetails.ChatterId] = DateTime.UtcNow;

            PublishLogMessage($"[{chatterDetails.ChatterName}] {commandHandler.CommandName} {commandArgs.Argument}");

            commandHandler.Execute(commandArgs);

            foreach (var message in commandHandler.ChatMessages)
            {
                OnChatMessageToSend?.Invoke(this, 
                    new ChatMessageEventArgs(chatterDetails.ChatterName, message));
            }

            if (commandHandler.StreamerBankrupted)
            {
                chatterDetails.Company.VictoryCount++;

                OnBankruptedStreamer?.Invoke(this,
                    new BankruptedStreamerArgs());
            }

            if (commandHandler.PlayerDataUpdated || 
                commandHandler.StreamerBankrupted)
            {
                OnPlayerDataUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}