using Moq;
using Xunit;
using ContactManagement.BLL.Concrete;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;

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
            
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();
            _countryRepositoryMock = new Mock<IRepository<Country>>();
            _specificCountryRepositoryMock = new Mock<ICountryRepository>();

           
            _repositoryFactoryMock.Setup(factory => factory.CreateRepository<Country>()).Returns(_countryRepositoryMock.Object);
            _repositoryFactoryMock.Setup(factory => factory.CreateCountryRepository()).Returns(_specificCountryRepositoryMock.Object);

            
            _countryService = new CountryService(_repositoryFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfCountries()
        {
            // Arrange: Create a list of countries to return when the method is called
            var countries = new List<Country>
            {
                new Country { CountryId = 1, CountryName = "United States" },
                new Country { CountryId = 2, CountryName = "Canada" }
            };

            // Mock the GetAllAsync method to return the list of countries
            _countryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(countries);

            // Act: Call the GetAllAsync method to retrieve the list of countries
            var result = await _countryService.GetAllAsync();

            // Assert: Verify that the result is not null
            Xunit.Assert.NotNull(result);

            // Verify that the result contains exactly 2 countries
            Xunit.Assert.Equal(2, result.Count);

            // Verify the names of the countries in the list
            Xunit.Assert.Equal("United States", result[0].CountryName);
            Xunit.Assert.Equal("Canada", result[1].CountryName);
        }


        [Fact]
        public async Task GetAsync_ValidCountryId_ReturnsCountry()
        {
            // Arrange: Create a mock country object to return when GetAsync is called with a valid country ID
            var country = new Country { CountryId = 1, CountryName = "United States" };

            // Mock the GetAsync method to return the mock country for ID 1
            _countryRepositoryMock.Setup(repo => repo.GetAsync(1)).ReturnsAsync(country);

            // Act: Call the GetAsync method to retrieve the country with ID 1
            var result = await _countryService.GetAsync(1);

            // Assert: Ensure that the result is not null
            Xunit.Assert.NotNull(result);

            // Assert: Verify that the country name in the result is "United States"
            Xunit.Assert.Equal("United States", result.CountryName);
        }


        [Fact]
        public async Task GetAsync_InvalidCountryId_ReturnsNull()
        {
            // Arrange: Mock the GetAsync method to return null when an invalid country ID (999) is provided
            _countryRepositoryMock.Setup(repo => repo.GetAsync(999)).ReturnsAsync((Country)null);

            // Act: Call the GetAsync method with an invalid country ID (999)
            var result = await _countryService.GetAsync(999);

            // Assert: Verify that the result is null
            Xunit.Assert.Null(result);
        }


        [Fact]
        public async Task SaveAsync_ValidCountry_ReturnsCountryId()
        {
            // Arrange: Mock the SaveAsync method to return the country ID (1) when a valid country object is saved
            var country = new Country { CountryId = 1, CountryName = "United States" };
            _countryRepositoryMock.Setup(repo => repo.SaveAsync(country)).ReturnsAsync(1);

            // Act: Call the SaveAsync method of the CountryService with a valid country object
            var result = await _countryService.SaveAsync(country);

            // Assert: Verify that the result is the expected country ID (1)
            Xunit.Assert.Equal(1, result);
        }


        [Fact]
        public async Task DeleteAsync_ValidCountryId_DeletesCountry()
        {
            // Arrange: Set up the mock repository to return a completed task when DeleteAsync is called with a valid countryId
            int countryId = 1;
            _countryRepositoryMock.Setup(repo => repo.DeleteAsync(countryId)).Returns(Task.CompletedTask);

            // Act: Call the DeleteAsync method of the CountryService with the valid countryId
            await _countryService.DeleteAsync(countryId);

            // Assert: Verify that the DeleteAsync method was called exactly once with the expected countryId
            _countryRepositoryMock.Verify(repo => repo.DeleteAsync(countryId), Times.Once);
        }


        [Fact]
        public async Task GetContactsWithCompanyAndCountry_ValidCountryId_ReturnsStatistics()
        {
            // Arrange: Set up the mock to return a dictionary of statistics when GetCompanyStatisticsByCountryId is called with the given countryId.
            var statistics = new Dictionary<string, int>
            {
                { "CompanyCount", 5 },
                { "ContactCount", 10 }
            };
            int countryId = 1;
            _specificCountryRepositoryMock.Setup(repo => repo.GetCompanyStatisticsByCountryId(countryId)).ReturnsAsync(statistics);

            // Act: Call the method in the service that retrieves the statistics for the given countryId.
            var result = await _countryService.GetCompanyStatisticsByCountryId(countryId);

            // Assert: Verify the result is not null, has the expected number of entries, and contains the correct values for CompanyCount and ContactCount.
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count); // Ensure the dictionary contains two items
            Xunit.Assert.Equal(5, result["CompanyCount"]); // Ensure the CompanyCount is 5
            Xunit.Assert.Equal(10, result["ContactCount"]); // Ensure the ContactCount is 10
        }


        [Fact]
        public async Task GetContactsWithCompanyAndCountry_InvalidCountryId_ReturnsEmptyStatistics()
        {
            // Arrange: Set up the mock to return an empty dictionary when GetCompanyStatisticsByCountryId is called with an invalid countryId (999).
            var statistics = new Dictionary<string, int>();
            int countryId = 999;
            _specificCountryRepositoryMock.Setup(repo => repo.GetCompanyStatisticsByCountryId(countryId)).ReturnsAsync(statistics);

            // Act: Call the method in the service that retrieves the statistics for the given invalid countryId.
            var result = await _countryService.GetCompanyStatisticsByCountryId(countryId);

            // Assert: Ensure the result is not null, and it's an empty dictionary.
            Xunit.Assert.NotNull(result); // The result should not be null
            Xunit.Assert.Empty(result); // The result should be empty (no statistics available for the invalid countryId)
        }

    }
}