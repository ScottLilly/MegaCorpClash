using MegaCorpClash.Models;

namespace MegaCorpClash.Services.CustomEventArgs;

public class BroadcasterCommandEventArgs : EventArgs
{
    public BroadcasterCommandType CommandType { get; }
    public int Value { get; }

    public BroadcasterCommandEventArgs(BroadcasterCommandType commandType, int value)
    {
        CommandType = commandType;
        Value = value;
    }
}
