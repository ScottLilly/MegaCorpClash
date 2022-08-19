namespace MegaCorpClash.Models.CustomEventArgs;

public class ChatMessageEventArgs
{
    public string ChatterDisplayName { get; }
    public string Message { get; }

    public ChatMessageEventArgs(string chatterDisplayName, string message)
    {
        ChatterDisplayName = chatterDisplayName;
        Message = message;
    }
}