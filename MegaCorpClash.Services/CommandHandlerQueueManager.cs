using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Services;

public class CommandHandlerQueueManager : 
    BaseQueueManager<(BaseCommandHandler, GameCommandArgs)>
{
    public CommandHandlerQueueManager()
    {
        Task.Factory.StartNew(Consumer);
    }

    private void Consumer()
    {
        foreach (var item in _queue.GetConsumingEnumerable())
        {
            item.Item1.Execute(item.Item2);
            Console.WriteLine(item.Item1.CommandName);
        }
    }
}