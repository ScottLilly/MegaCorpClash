using Newtonsoft.Json.Converters;

namespace MegaCorpClash.Models;

public sealed class EmployeeQuantity
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))] 
    public EmployeeType Type { get; set; }
    public int Quantity { get; set; } = 1;
}