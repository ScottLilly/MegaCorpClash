using LiteDB;
using MegaCorpClash.Models;

namespace MegaCorpClash.Services.Persistence;

public class CompanyRepository : IRepository, IDisposable
{
    const string COMPANIES_COLLECTION = "companies";

    readonly static LiteDatabase _database;

    static CompanyRepository()
    {
        Directory.CreateDirectory(@".\Data");

        _database = new LiteDatabase(@".\Data\Companies.db");
    }
    private CompanyRepository()
    {
    }

    public static IRepository GetInstance()
    {
        return new CompanyRepository();
    }

    public void AddCompany(Company company)
    {
        _database.GetCollection<Company>(COMPANIES_COLLECTION).Insert(company);
    }

    public void ChangeMotto(string chatterId, string motto)
    {
        var company = 
            _database.GetCollection<Company>(COMPANIES_COLLECTION)
            .FindOne(c => c.UserId == chatterId);

        company.Motto = motto;

        _database.GetCollection<Company>(COMPANIES_COLLECTION)
            .Update(company);
    }

    public IEnumerable<Company> GetAllCompanies()
    {
        return _database.GetCollection<Company>(COMPANIES_COLLECTION).FindAll();
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}