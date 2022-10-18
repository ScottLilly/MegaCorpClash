using MegaCorpClash.Models.CustomEventArgs;
using System.Collections.Concurrent;

namespace MegaCorpClash.Services;

public abstract class BaseQueueManager<T>
{
    protected readonly BlockingCollection<T> _queue = new();

    public event EventHandler<LogMessageEventArgs> OnLogMessagePublished;

    public void Add(T obj)
    {
        _queue.Add(obj);
    }

    public void Stop()
    {
        _queue.CompleteAdding();
    }

    #region Protected helper functions

    protected void PublishLogMessage(string message)
    {
        OnLogMessagePublished.Invoke(this, new LogMessageEventArgs(message));
    }

    #endregion
}