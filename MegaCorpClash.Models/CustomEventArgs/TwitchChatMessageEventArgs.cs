namespace MegaCorpClash.Models.CustomEventArgs;

public class TwitchChatMessageEventArgs
{
    public string ChatterDisplayName { get; }
    public string Message { get; }

    public TwitchChatMessageEventArgs(string chatterDisplayName, string message)
    {
        ChatterDisplayName = chatterDisplayName;
        Message = message;
    }
}