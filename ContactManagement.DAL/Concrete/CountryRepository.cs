using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.DAL.Concrete
{
    public class CountryRepository : ICountryRepository
    {

        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves all countries from the database.
        /// </summary>
        /// <returns>A list of all countries.</returns>
        public async Task<List<Country>> GetAllAsync()
        {
            return await _context.Countries.ToListAsync();
        }


        /// <summary>
        /// Asynchronously retrieves a country by its ID from the database.
        /// </summary>
        /// <param name="countryId">The ID of the country to retrieve.</param>
        /// <returns>The country with the specified ID, or null if not found.</returns>
        public async Task<Country> GetAsync(int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId);

            return country;
        }

        /// <summary>
        /// Asynchronously saves a country to the database. If the country exists, it updates it; otherwise, it adds a new country.
        /// </summary>
        /// <param name="country">The country entity to save.</param>
        /// <returns>The ID of the saved country.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the country exists but the ID is not found in the database.</exception>
        public async Task<int> SaveAsync(Country country)
        {
            if (country.CountryId > 0)
            {
                var exists = await _context.Countries.AnyAsync(c => c.CountryId == country.CountryId);
                if (!exists)
                {
                    throw new KeyNotFoundException($"Country with ID {country.CountryId} not found.");
                }
            }


            _context.Entry(country).State = country.CountryId > 0 ? EntityState.Modified : EntityState.Added;
            await _context.SaveChangesAsync();

            return country.CountryId;
        }

        /// <summary>
        /// Asynchronously deletes a country from the database by its ID.
        /// </summary>
        /// <param name="countryId">The ID of the country to delete.</param>
        /// <exception cref="KeyNotFoundException">Thrown if no country with the specified ID is found in the database.</exception>
        public async Task DeleteAsync(int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId)
                ?? throw new KeyNotFoundException($"Country with ID {countryId} not found.");

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Retrieves company statistics by country ID, including the count of contacts for each company in the specified country.
        /// </summary>
        /// <param name="countryId">The ID of the country for which to retrieve company statistics.</param>
        /// <returns>A dictionary where the key is the company name and the value is the count of contacts associated with that company in the specified country.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no contacts are found for the specified country.</exception>
        public async Task<Dictionary<string, int>> GetCompanyStatisticsByCountryId(int countryId)
        {
            var contacts = await _context.Contacts
                .Where(c => c.CountryId == countryId && c.Company != null)
                .Include(c => c.Company)
                .ToListAsync(); 

            return contacts
                .Where(c => c.Company != null)
                .Select(c => c.Company!)
                .GroupBy(company => company.CompanyName)
                .ToDictionary(g => g.Key, g => g.Count());
        }

    }
}
