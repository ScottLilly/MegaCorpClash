namespace MegaCorpClash.Models.CustomEventArgs;

public sealed class ChattedEventArgs : EventArgs
{
    public string UserId { get; }

    public ChattedEventArgs(string userId)
    {
        UserId = userId;
    }
}