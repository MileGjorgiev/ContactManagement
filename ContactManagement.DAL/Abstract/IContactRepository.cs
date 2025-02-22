using ContactManagement.Models.Entities;

namespace ContactManagement.DAL.Abstract
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<List<Contact>> GetContactsWithCompanyAndCountry();
        Task<List<Contact>> FilterContacts(int? countryId, int? companyId);
    }
}
