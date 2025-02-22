using Moq;
using Xunit;
using ContactManagement.BLL.Concrete;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactManagment.Tests.ServiceTests
{
    public class CountryServiceTests
    {
        private readonly Mock<IRepositoryFactory> _repositoryFactoryMock;
        private readonly Mock<IRepository<Country>> _countryRepositoryMock;
        private readonly Mock<ICountryRepository> _specificCountryRepositoryMock;
        private readonly CountryService _countryService;

        public CountryServiceTests()
        {
            // Initialize mocks
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();
            _countryRepositoryMock = new Mock<IRepository<Country>>();
            _specificCountryRepositoryMock = new Mock<ICountryRepository>();

            // Setup the repository factory to return the mocked repositories
            _repositoryFactoryMock.Setup(factory => factory.CreateRepository<Country>()).Returns(_countryRepositoryMock.Object);
            _repositoryFactoryMock.Setup(factory => factory.CreateCountryRepository()).Returns(_specificCountryRepositoryMock.Object);

            // Initialize the service with the mocked repository factory
            _countryService = new CountryService(_repositoryFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfCountries()
        {
            // Arrange
            var countries = new List<Country>
            {
                new Country { CountryId = 1, CountryName = "United States" },
                new Country { CountryId = 2, CountryName = "Canada" }
            };
            _countryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(countries);

            // Act
            var result = await _countryService.GetAllAsync();

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("United States", result[0].CountryName);
            Xunit.Assert.Equal("Canada", result[1].CountryName);
        }

        [Fact]
        public async Task GetAsync_ValidCountryId_ReturnsCountry()
        {
            // Arrange
            var country = new Country { CountryId = 1, CountryName = "United States" };
            _countryRepositoryMock.Setup(repo => repo.GetAsync(1)).ReturnsAsync(country);

            // Act
            var result = await _countryService.GetAsync(1);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("United States", result.CountryName);
        }

        [Fact]
        public async Task GetAsync_InvalidCountryId_ReturnsNull()
        {
            // Arrange
            _countryRepositoryMock.Setup(repo => repo.GetAsync(999)).ReturnsAsync((Country)null);

            // Act
            var result = await _countryService.GetAsync(999);

            // Assert
            Xunit.Assert.Null(result);
        }

        [Fact]
        public async Task SaveAsync_ValidCountry_ReturnsCountryId()
        {
            // Arrange
            var country = new Country { CountryId = 1, CountryName = "United States" };
            _countryRepositoryMock.Setup(repo => repo.SaveAsync(country)).ReturnsAsync(1);

            // Act
            var result = await _countryService.SaveAsync(country);

            // Assert
            Xunit.Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteAsync_ValidCountryId_DeletesCountry()
        {
            // Arrange
            int countryId = 1;
            _countryRepositoryMock.Setup(repo => repo.DeleteAsync(countryId)).Returns(Task.CompletedTask);

            // Act
            await _countryService.DeleteAsync(countryId);

            // Assert
            _countryRepositoryMock.Verify(repo => repo.DeleteAsync(countryId), Times.Once);
        }

        [Fact]
        public async Task GetContactsWithCompanyAndCountry_ValidCountryId_ReturnsStatistics()
        {
            // Arrange
            var statistics = new Dictionary<string, int>
            {
                { "CompanyCount", 5 },
                { "ContactCount", 10 }
            };
            int countryId = 1;
            _specificCountryRepositoryMock.Setup(repo => repo.GetContactsWithCompanyAndCountry(countryId)).ReturnsAsync(statistics);

            // Act
            var result = await _countryService.GetContactsWithCompanyAndCountry(countryId);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal(5, result["CompanyCount"]);
            Xunit.Assert.Equal(10, result["ContactCount"]);
        }

        [Fact]
        public async Task GetContactsWithCompanyAndCountry_InvalidCountryId_ReturnsEmptyStatistics()
        {
            // Arrange
            var statistics = new Dictionary<string, int>();
            int countryId = 999;
            _specificCountryRepositoryMock.Setup(repo => repo.GetContactsWithCompanyAndCountry(countryId)).ReturnsAsync(statistics);

            // Act
            var result = await _countryService.GetContactsWithCompanyAndCountry(countryId);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Empty(result);
        }
    }
}