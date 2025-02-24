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

        /// <summary>
        /// Creates and configures a new instance of the <see cref="AppDbContext"/> using the default connection string.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="AppDbContext"/> configured with the connection string specified in the application's settings.
        /// </returns>
        private AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(_databaseSettings.DefaultConnection);
            return new AppDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// Asynchronously retrieves all companies from the database.
        /// </summary>
        /// <returns>
        /// A list of <see cref="Company"/> objects representing all companies in the database.
        /// </returns>
        public async Task<List<Company>> GetAllAsync()
        {
            using (var context = CreateDbContext())
            {
                return await context.Companies.ToListAsync();
            }
        }

        /// <summary>
        /// Asynchronously retrieves a company from the database by its ID.
        /// </summary>
        /// <param name="companyId">The ID of the company to retrieve.</param>
        /// <returns>
        /// The <see cref="Company"/> object with the specified ID, or <c>null</c> if not found.
        /// </returns>
        public async Task<Company> GetAsync(int companyId)
        {
            using (var context = CreateDbContext())
            {
                var company = await context.Companies.FindAsync(companyId);

                LoggerSingleton.Instance.Log("User fetched successfully.");

                return company;
            }
        }

        /// <summary>
        /// Asynchronously saves a company to the database. If the company exists, it updates it; otherwise, it adds a new company.
        /// </summary>
        /// <param name="company">The company to be saved or updated.</param>
        /// <returns>
        /// The ID of the saved or updated company.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when trying to update a company that does not exist in the database.
        /// </exception>
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

        /// <summary>
        /// Asynchronously deletes a company from the database by its ID.
        /// </summary>
        /// <param name="companyId">The ID of the company to be deleted.</param>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the company with the specified ID is not found in the database.
        /// </exception>
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

        /// <summary>
        /// Asynchronously retrieves a paginated list of companies from the database.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of companies per page.</param>
        /// <returns>A list of companies for the specified page.</returns>
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

        /// <summary>
        /// Asynchronously retrieves the total number of companies in the database.
        /// </summary>
        /// <returns>The total number of companies.</returns>
        public async Task<int> GetTotalCompanies()
        {
            using (var context = CreateDbContext())
            {
                return await context.Companies.CountAsync();
            }
        }
    }
}
