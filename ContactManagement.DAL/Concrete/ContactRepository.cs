using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.DAL.Concrete
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;

        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts.ToListAsync();
        }

        public async Task<Contact> GetAsync(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);

            return contact;
        }

        public async Task<int> SaveAsync(Contact contact)
        {
            
            var companyExists = await _context.Companies.AnyAsync(c => c.CompanyId == contact.CompanyId);
            if (!companyExists)
            {
                throw new KeyNotFoundException($"Company with ID {contact.CompanyId} does not exist.");
            }

            
            var countryExists = await _context.Countries.AnyAsync(c => c.CountryId == contact.CountryId);
            if (!countryExists)
            {
                throw new KeyNotFoundException($"Country with ID {contact.CountryId} does not exist.");
            }

            
            if (contact.ContactId > 0)
            {
                var contactExists = await _context.Contacts.AnyAsync(c => c.ContactId == contact.ContactId);
                if (!contactExists)
                {
                    throw new KeyNotFoundException($"Contact with ID {contact.ContactId} not found.");
                }
            }

           
            _context.Entry(contact).State = contact.ContactId > 0 ? EntityState.Modified : EntityState.Added;
            await _context.SaveChangesAsync();

            return contact.ContactId;
        }

        public async Task DeleteAsync(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId)
                ?? throw new KeyNotFoundException($"Contact with ID {contactId} not found.");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

        }

        public async Task<List<Contact>> GetContactsWithCompanyAndCountry()
        {
            return await _context.Contacts
                .Include(c => c.Company)
                .Include(c => c.Country)
                .ToListAsync();
        }


        public async Task<List<Contact>> FilterContacts(int? countryId, int? companyId)
        {
            if (countryId.HasValue && countryId > 0)
            {
                bool countryExists = await _context.Countries.AnyAsync(c => c.CountryId == countryId);
                if (!countryExists)
                {
                    throw new KeyNotFoundException($"Country with ID {countryId} not found.");
                }
            }
            if (companyId.HasValue && companyId > 0)
            {
                bool companyExists = await _context.Companies.AnyAsync(c => c.CompanyId == companyId);
                if (!companyExists)
                {
                    throw new KeyNotFoundException($"Company with ID {companyId} not found.");
                }
            }


            return await _context.Contacts
                .Where(c => (!countryId.HasValue || c.CountryId == countryId) &&
                            (!companyId.HasValue || c.CompanyId == companyId))
                .ToListAsync();
        }

    }
}
