using ContactManagement.DAL.Abstract;
using ContactManagement.DAL.Configuration;
using ContactManagement.DAL.Singletons;
using ContactManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ContactManagement.DAL.Concrete
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DatabaseSettings _databaseSettings;

        public CompanyRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings.Value;
        }

        private AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(_databaseSettings.DefaultConnection);
            return new AppDbContext(optionsBuilder.Options);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            using (var context = CreateDbContext())
            {
                return await context.Companies.ToListAsync();
            }
        }

        public async Task<Company> GetAsync(int companyId)
        {
            using (var context = CreateDbContext())
            {
                var company = await context.Companies.FindAsync(companyId)
                      ?? throw new KeyNotFoundException($"Company with ID {companyId} not found.");

                LoggerSingleton.Instance.Log("User fetched successfully.");

                return company;
            }
        }

        public async Task<int> SaveAsync(Company company)
        {
            using (var context = CreateDbContext())
            {
                if (company.CompanyId > 0)
                {
                    var exists = await context.Companies.AnyAsync(c => c.CompanyId == company.CompanyId);
                    if (!exists)
                    {
                        throw new KeyNotFoundException($"Company with ID {company.CompanyId} not found.");
                    }
                }

                context.Entry(company).State = company.CompanyId > 0 ? EntityState.Modified : EntityState.Added;
                await context.SaveChangesAsync();

                return company.CompanyId;
            }
        }

        public async Task DeleteAsync(int companyId)
        {
            using (var context = CreateDbContext())
            {
                var company = await context.Companies.FindAsync(companyId)
                    ?? throw new KeyNotFoundException($"Company with ID {companyId} not found.");

                context.Companies.Remove(company);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Company>> GetCompaniesWithPagination(int pageNumber, int pageSize)
        {
            using (var context = CreateDbContext())
            {
                var skip = (pageNumber - 1) * pageSize;
                var companies = await context.Companies
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                return companies;
            }
        }

        public async Task<int> GetTotalCompanies()
        {
            using (var context = CreateDbContext())
            {
                return await context.Companies.CountAsync();
            }
        }
    }
}
