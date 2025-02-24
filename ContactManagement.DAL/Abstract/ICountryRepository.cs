using ContactManagement.Models.Entities;

namespace ContactManagement.DAL.Abstract
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<Dictionary<string, int>> GetCompanyStatisticsByCountryId(int countryId);
    }
}
