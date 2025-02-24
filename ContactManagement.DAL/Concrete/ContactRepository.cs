using ContactManagement.DAL.Abstract;
using ContactManagement.DAL.Singletons;
using ContactManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ContactManagement.DAL.Concrete
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;

        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves all contacts from the database.
        /// </summary>
        /// <returns>A list of all contacts.</returns>
        public async Task<List<Contact>> GetAllAsync()
        {
            return await _context.Contacts.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a contact by its ID from the database.
        /// </summary>
        /// <param name="contactId">The ID of the contact to retrieve.</param>
        /// <returns>The contact with the specified ID, or null if not found.</returns>
        public async Task<Contact> GetAsync(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);

            return contact;
        }

        /// <summary>
        /// Asynchronously saves a contact to the database. If the contact already exists, it updates it. Otherwise, it adds a new contact.
        /// </summary>
        /// <param name="contact">The contact to be saved or updated.</param>
        /// <returns>The ID of the saved or updated contact.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the company or country associated with the contact does not exist,
        /// or the contact to update is not found.</exception>
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

        /// <summary>
        /// Asynchronously deletes a contact from the database by its ID.
        /// </summary>
        /// <param name="contactId">The ID of the contact to be deleted.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the contact with the specified ID is not found in the database.</exception>
        public async Task DeleteAsync(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId)
                ?? throw new KeyNotFoundException($"Contact with ID {contactId} not found.");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Asynchronously retrieves a list of contacts along with their associated company and country information.
        /// </summary>
        /// <returns>A list of contacts with their corresponding company and country details.</returns>
        public async Task<List<Contact>> GetContactsWithCompanyAndCountry()
        {
            return await _context.Contacts
                .Include(c => c.Company)
                .Include(c => c.Country)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously filters contacts based on the specified country ID and company ID.
        /// </summary>
        /// <param name="countryId">The ID of the country to filter contacts by (optional).</param>
        /// <param name="companyId">The ID of the company to filter contacts by (optional).</param>
        /// <returns>A list of contacts that match the given country and company criteria.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the provided country ID or company ID does not exist in the database.</exception>
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
