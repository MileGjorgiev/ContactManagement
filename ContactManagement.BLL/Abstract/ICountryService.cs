using ContactManagement.Models.Entities;

namespace ContactManagement.BLL.Abstract
{
    public interface ICountryService
    {
        Task<List<Country>> GetAllAsync();
        Task<Country> GetAsync(int countryId);
        Task<int> SaveAsync(Country country);
        Task DeleteAsync(int countryId);
        Task<Dictionary<string, int>> GetCompanyStatisticsByCountryId(int countryId);
    }
}
