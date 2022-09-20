namespace MegaCorpClash.Models;

public sealed class GameCommand
{
    public string UserId { get; }
    public string DisplayName { get; }
    public string CommandName { get; }
    public string Argument { get; }

    public bool DoesNotHaveArguments =>
        string.IsNullOrWhiteSpace(Argument);

    public GameCommand(string userId, string displayName, 
        string commandName, string argument)
    {
        UserId = userId;
        DisplayName = displayName;
        CommandName = commandName;
        Argument = argument;
    }
}