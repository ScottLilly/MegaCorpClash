using Newtonsoft.Json.Converters;

namespace MegaCorpClash.Models;

public class EmployeeHiringDetails
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public EmployeeType Type { get; set; }
    public int CostToHire { get; set; }
}