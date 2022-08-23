namespace MegaCorpClash.Models.CustomEventArgs;

public class ChattedEventArgs : EventArgs
{
    public string UserId { get; }

    public ChattedEventArgs(string userId)
    {
        UserId = userId;
    }
}