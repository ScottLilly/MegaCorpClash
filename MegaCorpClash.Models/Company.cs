using Newtonsoft.Json;

namespace MegaCorpClash.Models;

public sealed class Company
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public bool IsBroadcaster { get; set; }
    public bool IsSubscriber { get; set; }
    public bool IsVip { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CompanyName { get; set; }
    public string Motto { get; set; } = "We don't need a motto";
    public int Points { get; set; }
    public int VictoryCount { get; set; }
    public List<EmployeeQuantity> Employees { get; set; } = new();

    [JsonIgnore]
    public string EmployeeList =>
        string.Join(", ",
            Employees
                .OrderBy(x => x.Type)
                .Select(x => $"{x.Quantity:N0} {x.Type}"));
}