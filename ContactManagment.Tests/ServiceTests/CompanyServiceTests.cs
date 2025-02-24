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
            
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();
            _companyRepositoryMock = new Mock<IRepository<Company>>();
            _specificCompanyRepositoryMock = new Mock<ICompanyRepository>();

           
            _repositoryFactoryMock.Setup(factory => factory.CreateRepository<Company>()).Returns(_companyRepositoryMock.Object);
            _repositoryFactoryMock.Setup(factory => factory.CreateCompanyRepository()).Returns(_specificCompanyRepositoryMock.Object);

           
            _companyService = new CompanyService(_repositoryFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfCompanies()
        {
            // Arrange: Create a list of companies to be returned by the mock repository
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, CompanyName = "Google" },
                new Company { CompanyId = 2, CompanyName = "Apple" }
            };

            // Arrange: Mock the repository to return the companies list when GetAllAsync is called
            _companyRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(companies);

            // Act: Call the GetAllAsync method from the service, which will internally use the mock repository
            var result = await _companyService.GetAllAsync();

            // Assert: Verify that the result is not null
            Xunit.Assert.NotNull(result);

            // Assert: Ensure the result contains exactly 2 companies
            Xunit.Assert.Equal(2, result.Count);

            // Assert: Verify that the first company's name is "Google"
            Xunit.Assert.Equal("Google", result[0].CompanyName);

            // Assert: Verify that the second company's name is "Apple"
            Xunit.Assert.Equal("Apple", result[1].CompanyName);
        }


        [Fact]
        public async Task GetAsync_ValidCompanyId_ReturnsCompany()
        {
            // Arrange: Create a company object that will be returned by the mock repository
            var company = new Company { CompanyId = 1, CompanyName = "Google" };

            // Arrange: Mock the repository to return the company object when GetAsync(1) is called
            _companyRepositoryMock.Setup(repo => repo.GetAsync(1)).ReturnsAsync(company);

            // Act: Call the GetAsync method from the service, which will internally use the mock repository
            var result = await _companyService.GetAsync(1);

            // Assert: Verify that the result is not null
            Xunit.Assert.NotNull(result);

            // Assert: Verify that the returned company's name matches "Google"
            Xunit.Assert.Equal("Google", result.CompanyName);
        }


        [Fact]
        public async Task GetAsync_InvalidCompanyId_ReturnsNull()
        {
            // Arrange: Mock the repository to return null when trying to get a company with an invalid ID (999)
            _companyRepositoryMock.Setup(repo => repo.GetAsync(999)).ReturnsAsync((Company)null);

            // Act: Call the GetAsync method from the service with the invalid company ID
            var result = await _companyService.GetAsync(999);

            // Assert: Verify that the result is null because no company was found for the given ID
            Xunit.Assert.Null(result);
        }


        [Fact]
        public async Task SaveAsync_ValidCompany_ReturnsCompanyId()
        {
            // Arrange: Create a company object and mock the repository's SaveAsync method to return the company ID (1)
            var company = new Company { CompanyId = 1, CompanyName = "Google" };
            _companyRepositoryMock.Setup(repo => repo.SaveAsync(company)).ReturnsAsync(1);

            // Act: Call the SaveAsync method from the service
            var result = await _companyService.SaveAsync(company);

            // Assert: Verify that the returned company ID is 1, which is the expected outcome
            Xunit.Assert.Equal(1, result);
        }


        [Fact]
        public async Task DeleteAsync_ValidCompanyId_DeletesCompany()
        {
            // Arrange: Set up the company ID and mock the repository's DeleteAsync method to complete successfully
            int companyId = 1;
            _companyRepositoryMock.Setup(repo => repo.DeleteAsync(companyId)).Returns(Task.CompletedTask);

            // Act: Call the DeleteAsync method from the service to delete the company
            await _companyService.DeleteAsync(companyId);

            // Assert: Verify that the DeleteAsync method was called exactly once with the correct companyId
            _companyRepositoryMock.Verify(repo => repo.DeleteAsync(companyId), Times.Once);
        }


        [Fact]
        public async Task GetCompaniesWithPagination_ValidParameters_ReturnsPaginatedCompanies()
        {
            // Arrange: Set up a list of companies and mock the repository's GetCompaniesWithPagination method to return the companies
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, CompanyName = "Google" },
                new Company { CompanyId = 2, CompanyName = "Apple" }
            };
            int pageNumber = 1;
            int pageSize = 2;
            _specificCompanyRepositoryMock.Setup(repo => repo.GetCompaniesWithPagination(pageNumber, pageSize)).ReturnsAsync(companies);

            // Act: Call the GetCompaniesWithPagination method of the service with the given page number and page size
            var result = await _companyService.GetCompaniesWithPagination(pageNumber, pageSize);

            // Assert: Verify that the result is not null and contains the correct number of companies
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count()); // Check that there are 2 companies in the result
            Xunit.Assert.Equal("Google", result.First().CompanyName); // Ensure the first company is "Google"
            Xunit.Assert.Equal("Apple", result.Last().CompanyName); // Ensure the last company is "Apple"
        }


        [Fact]
        public async Task GetTotalCompanies_ReturnsTotalCount()
        {
            // Arrange: Set up the mock to return a total company count of 10 when GetTotalCompanies is called
            int totalCompanies = 10;
            _specificCompanyRepositoryMock.Setup(repo => repo.GetTotalCompanies()).ReturnsAsync(totalCompanies);

            // Act: Call the GetTotalCompanies method from the service
            var result = await _companyService.GetTotalCompanies();

            // Assert: Verify that the result is equal to the expected total count of companies (10)
            Xunit.Assert.Equal(totalCompanies, result);
        }

    }
}