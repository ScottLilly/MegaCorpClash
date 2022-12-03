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
    Bonus
}

[Flags]
public enum LogMessageDestination
{
    None = 0,
    Console = 1,
    Disk = 2
}
