using ContactManagement.DAL.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManagement.DAL.Concrete
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IRepository<T> CreateRepository<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<IRepository<T>>();
        }

        public ICompanyRepository CreateCompanyRepository()
        {
            return _serviceProvider.GetRequiredService<ICompanyRepository>();
        }

        public IContactRepository CreateContactRepository()
        {
            return _serviceProvider.GetRequiredService<IContactRepository>();
        }

        public ICountryRepository CreateCountryRepository()
        {
            return _serviceProvider.GetRequiredService<ICountryRepository>();
        }
    }
}
