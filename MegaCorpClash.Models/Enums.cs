namespace MegaCorpClash.Models;

public enum EmployeeType
{
    Production,
    Sales,
    Marketing,
    Research,
    HR,
    Legal,
    Security,
    IT,
    Spy
}

public enum BroadcasterCommandType
{
    None,
    Bonus,
    Multiplier
}

[Flags]
public enum LogMessageDestination
{
    None = 0,
    Console = 1,
    Disk = 2
}
