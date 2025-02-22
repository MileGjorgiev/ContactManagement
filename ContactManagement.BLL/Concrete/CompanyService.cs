using ContactManagement.BLL.Abstract;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;

namespace ContactManagement.BLL.Concrete
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public CompanyService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            var companyRepository = _repositoryFactory.CreateRepository<Company>();
            return await companyRepository.GetAllAsync();
        }

        public async Task<Company> GetAsync(int companyId)
        {
            var companyRepository = _repositoryFactory.CreateRepository<Company>();
            return await companyRepository.GetAsync(companyId);
        }

        public async Task<int> SaveAsync(Company company)
        {
            var companyRepository = _repositoryFactory.CreateRepository<Company>();
            return await companyRepository.SaveAsync(company);
        }

        public async Task DeleteAsync(int companyId)
        {
            var companyRepository = _repositoryFactory.CreateRepository<Company>();
            await companyRepository.DeleteAsync(companyId);
        }

        public async Task<IEnumerable<Company>> GetCompaniesWithPagination(int pageNumber, int pageSize)
        {
            var companyRepository = _repositoryFactory.CreateCompanyRepository();
            return await companyRepository.GetCompaniesWithPagination(pageNumber,pageSize);
        }

        public async Task<int> GetTotalCompanies()
        {
            var companyRepository = _repositoryFactory.CreateCompanyRepository();
            return await companyRepository.GetTotalCompanies();
        }
    }
}
