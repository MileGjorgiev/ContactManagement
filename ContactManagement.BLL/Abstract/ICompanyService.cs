using ContactManagement.Models.Entities;

namespace ContactManagement.BLL.Abstract
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllAsync();
        Task<Company> GetAsync(int companyId);
        Task<int> SaveAsync(Company company);
        Task DeleteAsync(int companyId);
        Task<IEnumerable<Company>> GetCompaniesWithPagination(int pageNumber, int pageSize);
        Task<int> GetTotalCompanies();

    }
}
