using CSharpExtender.ExtensionMethods;
using LiteDB;
using MegaCorpClash.Models;

namespace MegaCorpClash.Services.Persistence;

public class CompanyRepository : IRepository, IDisposable
{
    const string COMPANIES_COLLECTION = "companies";

    readonly static LiteDatabase _database;

    static CompanyRepository()
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        var database = Path.Combine(dir, "Companies.db");

        Directory.CreateDirectory(dir);
        _database = new LiteDatabase(database);
    }
    private CompanyRepository()
    {
    }

    public static IRepository GetInstance()
    {
        return new CompanyRepository();
    }

    public void AddCompany(Company company) => 
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
        .Insert(company);

    public void UpdateCompany(Company company) =>
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
        .Update(company);

    public Company GetBroadcasterCompany() => 
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
            .FindOne(c => c.IsBroadcaster);

    public Company GetCompany(string companyId) => 
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
        .FindOne(c => c.UserId == companyId);

    public void AddPoints(string chatterId, int points)
    {
        var company = GetChatterCompany(chatterId);

        company.Points += points;

        UpdateCompany(company);
    }

    public void SubtractPoints(string chatterId, int points)
    {
        var company = GetChatterCompany(chatterId);

        company.Points -= points;

        UpdateCompany(company);
    }

    public bool OtherCompanyIsNamed(string chatterId, string newName)
    {
        return _database.GetCollection<Company>(COMPANIES_COLLECTION)
                    .Count(c => c.UserId != chatterId && 
                    c.CompanyName.ToLower() == newName.ToLower()) > 0;
    }

    public void ChangeCompanyName(string chatterId, string newName)
    {
        var company = GetChatterCompany(chatterId);

        company.CompanyName = newName;

        UpdateCompany(company);
    }

    public void IncrementVictoryCount(string chatterId)
    {
        var company = GetChatterCompany(chatterId);
        company.VictoryCount++;
        UpdateCompany(company);
    }
    
    public void ChangeMotto(string chatterId, string motto)
    {
        var company = GetChatterCompany(chatterId);

        company.Motto = motto;

        UpdateCompany(company);
    }

    public IEnumerable<Company> GetAllCompanies() => 
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
        .FindAll();

    public IEnumerable<Company> GetRichestCompanies(int count) => 
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
        .FindAll()
        .OrderByDescending(c => c.Points)
        .Take(count);

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

        UpdateCompany(company);
    }

    public void RemoveEmployeeOfType(string chatterId, EmployeeType type, 
        int quantity = 1)
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

        UpdateCompany(company);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _database.Dispose();
    }

    private static Company GetChatterCompany(string chatterId) =>
        _database.GetCollection<Company>(COMPANIES_COLLECTION)
        .FindOne(c => c.UserId == chatterId);
}