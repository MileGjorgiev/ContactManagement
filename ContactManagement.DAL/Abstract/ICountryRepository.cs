using ContactManagement.Models.Entities;

namespace ContactManagement.DAL.Abstract
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<Dictionary<string, int>> GetContactsWithCompanyAndCountry(int countryId);
    }
}
