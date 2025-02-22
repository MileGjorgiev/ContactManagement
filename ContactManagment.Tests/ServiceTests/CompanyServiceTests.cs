using Moq;
using Xunit;
using ContactManagement.BLL.Concrete;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactManagment.Tests.ServiceTests
{
    public class CompanyServiceTests
    {
        private readonly Mock<IRepositoryFactory> _repositoryFactoryMock;
        private readonly Mock<IRepository<Company>> _companyRepositoryMock;
        private readonly Mock<ICompanyRepository> _specificCompanyRepositoryMock;
        private readonly CompanyService _companyService;

        public CompanyServiceTests()
        {
            // Initialize mocks
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();
            _companyRepositoryMock = new Mock<IRepository<Company>>();
            _specificCompanyRepositoryMock = new Mock<ICompanyRepository>();

            // Setup the repository factory to return the mocked repositories
            _repositoryFactoryMock.Setup(factory => factory.CreateRepository<Company>()).Returns(_companyRepositoryMock.Object);
            _repositoryFactoryMock.Setup(factory => factory.CreateCompanyRepository()).Returns(_specificCompanyRepositoryMock.Object);

            // Initialize the service with the mocked repository factory
            _companyService = new CompanyService(_repositoryFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, CompanyName = "Google" },
                new Company { CompanyId = 2, CompanyName = "Apple" }
            };
            _companyRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(companies);

            // Act
            var result = await _companyService.GetAllAsync();

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("Google", result[0].CompanyName);
            Xunit.Assert.Equal("Apple", result[1].CompanyName);
        }

        [Fact]
        public async Task GetAsync_ValidCompanyId_ReturnsCompany()
        {
            // Arrange
            var company = new Company { CompanyId = 1, CompanyName = "Google" };
            _companyRepositoryMock.Setup(repo => repo.GetAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _companyService.GetAsync(1);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("Google", result.CompanyName);
        }

        [Fact]
        public async Task GetAsync_InvalidCompanyId_ReturnsNull()
        {
            // Arrange
            _companyRepositoryMock.Setup(repo => repo.GetAsync(999)).ReturnsAsync((Company)null);

            // Act
            var result = await _companyService.GetAsync(999);

            // Assert
            Xunit.Assert.Null(result);
        }

        [Fact]
        public async Task SaveAsync_ValidCompany_ReturnsCompanyId()
        {
            // Arrange
            var company = new Company { CompanyId = 1, CompanyName = "Google" };
            _companyRepositoryMock.Setup(repo => repo.SaveAsync(company)).ReturnsAsync(1);

            // Act
            var result = await _companyService.SaveAsync(company);

            // Assert
            Xunit.Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteAsync_ValidCompanyId_DeletesCompany()
        {
            // Arrange
            int companyId = 1;
            _companyRepositoryMock.Setup(repo => repo.DeleteAsync(companyId)).Returns(Task.CompletedTask);

            // Act
            await _companyService.DeleteAsync(companyId);

            // Assert
            _companyRepositoryMock.Verify(repo => repo.DeleteAsync(companyId), Times.Once);
        }

        [Fact]
        public async Task GetCompaniesWithPagination_ValidParameters_ReturnsPaginatedCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, CompanyName = "Google" },
                new Company { CompanyId = 2, CompanyName = "Apple" }
            };
            int pageNumber = 1;
            int pageSize = 2;
            _specificCompanyRepositoryMock.Setup(repo => repo.GetCompaniesWithPagination(pageNumber, pageSize)).ReturnsAsync(companies);

            // Act
            var result = await _companyService.GetCompaniesWithPagination(pageNumber, pageSize);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count());
            Xunit.Assert.Equal("Google", result.First().CompanyName);
            Xunit.Assert.Equal("Apple", result.Last().CompanyName);
        }

        [Fact]
        public async Task GetTotalCompanies_ReturnsTotalCount()
        {
            // Arrange
            int totalCompanies = 10;
            _specificCompanyRepositoryMock.Setup(repo => repo.GetTotalCompanies()).ReturnsAsync(totalCompanies);

            // Act
            var result = await _companyService.GetTotalCompanies();

            // Assert
            Xunit.Assert.Equal(totalCompanies, result);
        }
    }
}