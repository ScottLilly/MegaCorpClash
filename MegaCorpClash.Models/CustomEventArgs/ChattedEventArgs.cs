namespace MegaCorpClash.Models.CustomEventArgs;

public class ChattedEventArgs : EventArgs
{
    public string UserId { get; }
    public string DisplayName { get; }
    public bool IsBroadcaster { get; }
    public bool IsSubscriber { get; }
    public bool IsVip { get; }
    public bool IsNoisy { get; }

    public ChattedEventArgs(string userId, string displayName,
        bool isBroadcaster, bool isSubscriber, bool isVip, bool isNoisy)
    {
        UserId = userId;
        DisplayName = displayName;
        IsBroadcaster = isBroadcaster;
        IsSubscriber = isSubscriber;
        IsVip = isVip;
        IsNoisy = isNoisy;
    }
}