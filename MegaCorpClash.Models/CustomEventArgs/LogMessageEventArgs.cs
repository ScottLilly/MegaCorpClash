namespace MegaCorpClash.Models.CustomEventArgs;

public class LogMessageEventArgs : EventArgs
{
    public string Message { get; }
    public LogMessageDestination Destination { get; }

    public LogMessageEventArgs(string message,
        LogMessageDestination destination = LogMessageDestination.Disk)
    {
        Message = message;
        Destination = destination;
    }
}