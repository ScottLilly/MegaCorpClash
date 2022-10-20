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

[Flags]
public enum LogMessageDestination
{
    None = 0,
    Console = 1,
    Disk = 2
}
