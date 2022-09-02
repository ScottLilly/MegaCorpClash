namespace MegaCorpClash.Models;

public class Player
{
    public string Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string DisplayName { get; set; }
    public string CompanyName { get; set; }
    public string Motto { get; set; } = "We don't need a motto";
    public int Points { get; set; }
    public List<Employee> Employees { get; set; } = new();

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
                .Select(x => $"{x.Job} ({x.Count})"));
}