namespace MegaCorpClash.Models.CustomEventArgs;

public sealed class GameCommandArgs : ChattedEventArgs
{
    public string CommandName { get; }
    public string Argument { get; }

    public bool DoesNotHaveArguments =>
        string.IsNullOrWhiteSpace(Argument);

    public GameCommandArgs(string userId, string displayName,
        string commandName, string argument) 
        : base(userId, displayName)
    {
        CommandName = commandName;
        Argument = argument;
    }
}