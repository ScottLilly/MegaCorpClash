using CSharpExtender.ExtensionMethods;
using MegaCorpClash.Models;
using MegaCorpClash.Services.Persistence;

namespace Test.MegaCorpClash.Services;

public class InMemoryRepository : IRepository
{
    private readonly List<Company> _companies = new();

    public void AddCompany(Company company)
    {
        _companies.Add(company);
    }

    public void AddPoints(string chatterId, int points)
    {
        _companies.First(c => c.UserId == chatterId).Points += points;
    }

    public void IncrementVictoryCount(string chatterId)
    {
        _companies.First(c => c.UserId == chatterId).VictoryCount++;
    }

    public void ChangeCompanyName(string chatterId, string name)
    {
        _companies.First(c => c.UserId == chatterId).CompanyName = name;
    }

    public void ChangeMotto(string chatterId, string motto)
    {
        _companies.First(c => c.UserId == chatterId).Motto = motto;
    }

    public IEnumerable<Company> GetAllCompanies()
    {
        return _companies;
    }

    public Company GetBroadcasterCompany()
    {
        return _companies.First(c => c.IsBroadcaster);
    }

    public Company GetCompany(string companyId)
    {
        return _companies.First(c => c.UserId.Equals(companyId));
    }

    public IEnumerable<Company> GetRichestCompanies(int count)
    {
        return _companies.OrderByDescending(c => c.Points).Take(count);
    }

    public void HireEmployees(string chatterId, EmployeeType type, int quantity, int cost)
    {
        var company = GetChatterCompany(chatterId);

        company.Points -= cost;

        var empQtyObject =
            company.Employees.FirstOrDefault(e => e.Type == type);

        if (empQtyObject == null)
        {
            company.Employees
                .Add(new EmployeeQuantity
                {
                    Type = type,
                    Quantity = quantity
                });
        }
        else
        {
            empQtyObject.Quantity += quantity;
        }
    }

    public bool OtherCompanyIsNamed(string chatterId, string name)
    {
        return _companies.Any(c => c.UserId != chatterId && c.CompanyName.Matches(name));
    }

    public void RemoveEmployeeOfType(string chatterId, EmployeeType type, int quantity = 1)
    {
        var company = GetChatterCompany(chatterId);

        var employeeQuantity =
            company.Employees.FirstOrDefault(e => e.Type == type);

        if (employeeQuantity == null)
        {
            // TODO: Throw exception (?)
        }
        else if (employeeQuantity.Quantity <= quantity)
        {
            company.Employees.Remove(employeeQuantity);
        }
        else
        {
            employeeQuantity.Quantity -= quantity;
        }
    }

    public void SubtractPoints(string chatterId, int points)
    {
        _companies.First(c => c.UserId == chatterId).Points -= points;
    }

    public void UpdateCompany(Company company)
    {
        // May cause problem, due to tests using referenced objects
    }

    private Company GetChatterCompany(string chatterId) =>
        _companies.First(c => c.UserId == chatterId);
}