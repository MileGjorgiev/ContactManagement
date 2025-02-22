using ContactManagement.Models.Entities;

namespace ContactManagement.DAL.Abstract
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<IEnumerable<Company>> GetCompaniesWithPagination(int pageNumber, int pageSize);
        Task<int> GetTotalCompanies();
    }
}
