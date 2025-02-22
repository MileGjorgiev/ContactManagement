using ContactManagement.BLL.Abstract;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;

namespace ContactManagement.BLL.Concrete
{
    public class ContactService : IContactService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ContactService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            var contactRepository = _repositoryFactory.CreateRepository<Contact>();
            return await contactRepository.GetAllAsync();
        }

        public async Task<Contact> GetAsync(int contactId)
        {
            var contactRepository = _repositoryFactory.CreateRepository<Contact>();
            return await contactRepository.GetAsync(contactId);
        }

        public async Task<int> SaveAsync(Contact contact)
        {
            var contactRepository = _repositoryFactory.CreateRepository<Contact>();
            return await contactRepository.SaveAsync(contact);
        }

        public async Task DeleteAsync(int contactId)
        {
            var contactRepository = _repositoryFactory.CreateRepository<Contact>();
            await contactRepository.DeleteAsync(contactId);
        }

        public async Task<List<Contact>> GetContactsWithCompanyAndCountry()
        {
            var contactRepository = _repositoryFactory.CreateContactRepository();
            return await contactRepository.GetContactsWithCompanyAndCountry();
        }


        public async Task<List<Contact>> FilterContacts(int? countryId, int? companyId)
        {
            var contactRepository = _repositoryFactory.CreateContactRepository();
            return await contactRepository.FilterContacts(countryId, companyId);
        }

    }
}
