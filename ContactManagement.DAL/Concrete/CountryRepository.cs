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

        public async Task<List<Country>> GetAllAsync()
        {
            return await _context.Countries.ToListAsync();
        }

        public async Task<Country> GetAsync(int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId)
                  ?? throw new KeyNotFoundException($"Country with ID {countryId} not found.");

            return country;
        }

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

        public async Task DeleteAsync(int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId)
                ?? throw new KeyNotFoundException($"Country with ID {countryId} not found.");

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

        }

        public async Task<Dictionary<string, int>> GetContactsWithCompanyAndCountry(int countryId)
        {
            var country = await _context.Countries.FindAsync(countryId)
                  ?? throw new KeyNotFoundException($"Country with ID {countryId} not found.");

            return country.GetCompanyStatisticsByCountryId(countryId);
        }
    }
}
