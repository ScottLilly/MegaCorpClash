using System.Collections.Concurrent;

namespace MegaCorpClash.Services;

public abstract class BaseQueueManager<T>
{
    protected readonly BlockingCollection<T> _queue = new();

    public void Add(T obj)
    {
        _queue.Add(obj);
    }

    public void Stop()
    {
        _queue.CompleteAdding();
    }
}