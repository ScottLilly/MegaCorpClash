using Newtonsoft.Json;

namespace MegaCorpClash.Models;

public class Company
{
    public string ChatterId { get; set; }
    public bool IsBroadcaster { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ChatterName { get; set; }
    public string CompanyName { get; set; }
    public string Motto { get; set; } = "We don't need a motto";
    public int Points { get; set; }
    public List<Employee> Employees { get; set; } = new();

    [JsonIgnore]
    public string EmployeeList =>
        string.Join(", ",
            Employees
                .GroupBy(e => e.Type)
                .Select(g => new
                {
                    Job = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Job)
                .Select(x => $"{x.Count} {x.Job}"));
}