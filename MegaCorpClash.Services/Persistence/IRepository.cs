using MegaCorpClash.Models;

namespace MegaCorpClash.Services.Persistence;

public interface IRepository
{
    public void AddCompany(Company company);
    public IEnumerable<Company> GetAllCompanies();
    public void ChangeMotto(string chatterId, string motto);
}
