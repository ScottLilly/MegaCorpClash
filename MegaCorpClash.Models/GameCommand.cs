namespace MegaCorpClash.Models;

public sealed class GameCommand
{
    public string ChatterId { get; }
    public string ChatterName { get; }
    public string CommandName { get; }
    public string Argument { get; }

    public bool DoesNotHaveArguments =>
        string.IsNullOrWhiteSpace(Argument);

    public GameCommand(string chatterId, string chatterName, 
        string commandName, string argument)
    {
        ChatterId = chatterId;
        ChatterName = chatterName;
        CommandName = commandName;
        Argument = argument;
    }
}