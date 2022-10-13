using System.Collections.Concurrent;

namespace MegaCorpClash.Services;

public class BaseQueueManager<T>
{
    private readonly ConcurrentQueue<T> _queue = new();

    public void Add(T obj)
    {
        _queue.Enqueue(obj);
    }
}