using Newtonsoft.Json.Converters;

namespace MegaCorpClash.Models;

public sealed class Employee
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))] 
    public EmployeeType Type { get; set; }
    public int SkillLevel { get; set; } = 1;
}