using ContactManagement.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagement.BLL.Abstract
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllAsync();
        Task<Contact> GetAsync(int contactId);
        Task<int> SaveAsync(Contact contact);
        Task DeleteAsync(int contactId);
        Task<List<Contact>> GetContactsWithCompanyAndCountry();
        Task<List<Contact>> FilterContacts(int? countryId, int? companyId);
    }
}
