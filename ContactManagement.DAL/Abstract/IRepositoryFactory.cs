namespace ContactManagement.DAL.Abstract
{
    public interface IRepositoryFactory
    {
        IRepository<T> CreateRepository<T>() where T : class;
        ICompanyRepository CreateCompanyRepository();
        IContactRepository CreateContactRepository();
        ICountryRepository CreateCountryRepository();
    }
}
