using System.Collections.Concurrent;

namespace MegaCorpClash.Services;

public abstract class BaseQueueManager<T>
{
    protected readonly ConcurrentQueue<T> _queue = new();

    public void Add(T obj)
    {
        _queue.Enqueue(obj);
    }

    public abstract void Execute(T command);
}