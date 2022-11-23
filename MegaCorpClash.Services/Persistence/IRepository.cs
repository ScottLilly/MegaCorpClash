using MegaCorpClash.Models;

namespace MegaCorpClash.Services.Persistence;

public interface IRepository
{
    public void AddCompany(Company company);
    public void UpdateCompany(Company company);
    public Company GetBroadcasterCompany();
    public Company GetCompany(string companyId);
    public IEnumerable<Company> GetAllCompanies();
    public void ChangeMotto(string chatterId, string motto);
    public bool OtherCompanyIsNamed(string chatterId, string name);
    public void ChangeCompanyName(string chatterId, string name);
    public void IncrementVictoryCount(string chatterId);
    public IEnumerable<Company> GetRichestCompanies(int count);
    public void RemoveEmployeeOfType(string chatterId, EmployeeType type, int quantity = 1);
    public void AddPoints(string chatterId, int points);
    public void SubtractPoints(string chatterId, int points);
    public void HireEmployees(string chatterId, EmployeeType type, int quantity, int cost);
}