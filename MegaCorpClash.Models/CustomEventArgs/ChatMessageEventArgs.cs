namespace MegaCorpClash.Models.CustomEventArgs;

public sealed class ChatMessageEventArgs
{
    public string DisplayName { get; }
    public string Message { get; }

    public ChatMessageEventArgs(string displayName, string message)
    {
        DisplayName = displayName;
        Message = message;
    }
}