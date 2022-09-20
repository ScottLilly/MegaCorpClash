namespace MegaCorpClash.Models.CustomEventArgs;

public sealed class ChattedEventArgs : EventArgs
{
    public string UserId { get; }
    public string DisplayName { get; }

    public ChattedEventArgs(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }
}