namespace MegaCorpClash.Models.CustomEventArgs;

public class PublishMessageToTwitchChatEventArgs
{
    public string ChatterDisplayName { get; }
    public string Message { get; }

    public PublishMessageToTwitchChatEventArgs(string chatterDisplayName, string message)
    {
        ChatterDisplayName = chatterDisplayName;
        Message = message;
    }
}