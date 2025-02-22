using Moq;
using Xunit;
using ContactManagement.BLL.Concrete;
using ContactManagement.DAL.Abstract;
using ContactManagement.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactManagment.Tests.ServiceTests
{
    public class ContactServiceTests
    {
        private readonly Mock<IRepositoryFactory> _repositoryFactoryMock;
        private readonly Mock<IRepository<Contact>> _contactRepositoryMock;
        private readonly Mock<IContactRepository> _specificContactRepositoryMock;
        private readonly ContactService _contactService;

        public ContactServiceTests()
        {
            // Initialize mocks
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();
            _contactRepositoryMock = new Mock<IRepository<Contact>>();
            _specificContactRepositoryMock = new Mock<IContactRepository>();

            // Setup the repository factory to return the mocked repositories
            _repositoryFactoryMock.Setup(factory => factory.CreateRepository<Contact>()).Returns(_contactRepositoryMock.Object);
            _repositoryFactoryMock.Setup(factory => factory.CreateContactRepository()).Returns(_specificContactRepositoryMock.Object);

            // Initialize the service with the mocked repository factory
            _contactService = new ContactService(_repositoryFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 },
                new Contact { ContactId = 2, ContactName = "Jane Doe", CompanyId = 2, CountryId = 2 }
            };
            _contactRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(contacts);

            // Act
            var result = await _contactService.GetAllAsync();

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("John Doe", result[0].ContactName);
            Xunit.Assert.Equal("Jane Doe", result[1].ContactName);
        }

        [Fact]
        public async Task GetAsync_ValidContactId_ReturnsContact()
        {
            // Arrange
            var contact = new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 };
            _contactRepositoryMock.Setup(repo => repo.GetAsync(1)).ReturnsAsync(contact);

            // Act
            var result = await _contactService.GetAsync(1);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("John Doe", result.ContactName);
        }

        [Fact]
        public async Task GetAsync_InvalidContactId_ReturnsNull()
        {
            // Arrange
            _contactRepositoryMock.Setup(repo => repo.GetAsync(999)).ReturnsAsync((Contact)null);

            // Act
            var result = await _contactService.GetAsync(999);

            // Assert
            Xunit.Assert.Null(result);
        }

        [Fact]
        public async Task SaveAsync_ValidContact_ReturnsContactId()
        {
            // Arrange
            var contact = new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 };
            _contactRepositoryMock.Setup(repo => repo.SaveAsync(contact)).ReturnsAsync(1);

            // Act
            var result = await _contactService.SaveAsync(contact);

            // Assert
            Xunit.Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteAsync_ValidContactId_DeletesContact()
        {
            // Arrange
            int contactId = 1;
            _contactRepositoryMock.Setup(repo => repo.DeleteAsync(contactId)).Returns(Task.CompletedTask);

            // Act
            await _contactService.DeleteAsync(contactId);

            // Assert
            _contactRepositoryMock.Verify(repo => repo.DeleteAsync(contactId), Times.Once);
        }

        [Fact]
        public async Task GetContactsWithCompanyAndCountry_ReturnsListOfContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 },
                new Contact { ContactId = 2, ContactName = "Jane Doe", CompanyId = 2, CountryId = 2 }
            };
            _specificContactRepositoryMock.Setup(repo => repo.GetContactsWithCompanyAndCountry()).ReturnsAsync(contacts);

            // Act
            var result = await _contactService.GetContactsWithCompanyAndCountry();

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("John Doe", result[0].ContactName);
            Xunit.Assert.Equal("Jane Doe", result[1].ContactName);
        }

        [Fact]
        public async Task FilterContacts_ValidFilters_ReturnsFilteredContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
            };
            int companyId = 1;
            int countryId = 1;
            _specificContactRepositoryMock.Setup(repo => repo.FilterContacts(countryId, companyId)).ReturnsAsync(contacts);

            // Act
            var result = await _contactService.FilterContacts(countryId, companyId);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result);
            Xunit.Assert.Equal("John Doe", result[0].ContactName);
        }

        [Fact]
        public async Task FilterContacts_NoFilters_ReturnsAllContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 },
                new Contact { ContactId = 2, ContactName = "Jane Doe", CompanyId = 2, CountryId = 2 }
            };
            _specificContactRepositoryMock.Setup(repo => repo.FilterContacts(null, null)).ReturnsAsync(contacts);

            // Act
            var result = await _contactService.FilterContacts(null, null);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("John Doe", result[0].ContactName);
            Xunit.Assert.Equal("Jane Doe", result[1].ContactName);
        }
    }
}