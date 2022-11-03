using MegaCorpClash.Services.CustomEventArgs;
using System.Collections.Concurrent;

namespace MegaCorpClash.Services.Queues;

public abstract class BaseTypedQueue<T>
{
    protected static readonly NLog.Logger Logger =
        NLog.LogManager.GetCurrentClassLogger();

    protected readonly BlockingCollection<T> _queue = new();

    public event EventHandler<LogMessageEventArgs> OnLogMessagePublished;
    public event EventHandler<ChatMessageEventArgs> OnChatMessagePublished;

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
        OnLogMessagePublished.Invoke(this,
            new LogMessageEventArgs(message));
    }

    protected void PublishChatMessage(string chatterName, string message)
    {
        OnChatMessagePublished?.Invoke(this,
            new ChatMessageEventArgs(chatterName, message));
    }

    #endregion
}