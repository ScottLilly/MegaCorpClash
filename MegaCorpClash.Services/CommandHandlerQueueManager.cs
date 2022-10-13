using MegaCorpClash.Models.ChatCommandHandlers;
using MegaCorpClash.Models.CustomEventArgs;

namespace MegaCorpClash.Services;

public class CommandHandlerQueueManager : 
    BaseQueueManager<(BaseCommandHandler, GameCommandArgs)>
{
    public void RunItemFromQueue()
    {
        if (_queue.TryDequeue(out var queued))
        {
            Execute(queued);
        }
    }

    public override void Execute((BaseCommandHandler, GameCommandArgs) command)
    {
        command.Item1.Execute(command.Item2);
    }
}