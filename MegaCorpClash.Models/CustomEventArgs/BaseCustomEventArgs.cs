namespace MegaCorpClash.Models.CustomEventArgs;

public abstract class BaseCustomEventArgs : EventArgs
{
    public string TwitchId { get; }
    public string TwitchDisplayName { get; }

    protected BaseCustomEventArgs(string twitchId, string twitchDisplayName)
    {
        TwitchId = twitchId;
        TwitchDisplayName = twitchDisplayName;
    }
}