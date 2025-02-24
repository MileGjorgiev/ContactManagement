using ContactManagement.BLL.Abstract;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;

namespace ContactManagement.BLL.Concrete
{
    public class CountryService : ICountryService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public CountryService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<Country>> GetAllAsync()
        {
            var countryRepository = _repositoryFactory.CreateRepository<Country>();
            return await countryRepository.GetAllAsync();
        }

        public async Task<Country> GetAsync(int countryId)
        {
            var countryRepository = _repositoryFactory.CreateRepository<Country>();
            return await countryRepository.GetAsync(countryId);
        }

        public async Task<int> SaveAsync(Country country)
        {
            var countryRepository = _repositoryFactory.CreateRepository<Country>();
            return await countryRepository.SaveAsync(country);
        }

        public async Task DeleteAsync(int countryId)
        {
            var countryRepository = _repositoryFactory.CreateRepository<Country>();
            await countryRepository.DeleteAsync(countryId);
        }

        public async Task<Dictionary<string, int>> GetCompanyStatisticsByCountryId(int countryId)
        {
            var countryRepository = _repositoryFactory.CreateCountryRepository();
            return await countryRepository.GetCompanyStatisticsByCountryId(countryId);
        }
    }
}
