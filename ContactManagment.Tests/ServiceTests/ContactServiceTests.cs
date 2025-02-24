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
           
            _repositoryFactoryMock = new Mock<IRepositoryFactory>();
            _contactRepositoryMock = new Mock<IRepository<Contact>>();
            _specificContactRepositoryMock = new Mock<IContactRepository>();

            
            _repositoryFactoryMock.Setup(factory => factory.CreateRepository<Contact>()).Returns(_contactRepositoryMock.Object);
            _repositoryFactoryMock.Setup(factory => factory.CreateContactRepository()).Returns(_specificContactRepositoryMock.Object);

           
            _contactService = new ContactService(_repositoryFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfContacts()
        {
            // Arrange: Set up the mock to return a list of contacts when GetAllAsync is called
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 },
                new Contact { ContactId = 2, ContactName = "Jane Doe", CompanyId = 2, CountryId = 2 }
            };
            _contactRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(contacts);

            // Act: Call the GetAllAsync method from the service to get the contacts
            var result = await _contactService.GetAllAsync();

            // Assert: Verify that the result is not null, contains two contacts, and that the contact names are as expected
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count); // Ensure that the returned list contains 2 contacts
            Xunit.Assert.Equal("John Doe", result[0].ContactName); // Verify the name of the first contact
            Xunit.Assert.Equal("Jane Doe", result[1].ContactName); // Verify the name of the second contact
        }


        [Fact]
        public async Task GetAsync_ValidContactId_ReturnsContact()
        {
            // Arrange: Set up the mock to return a specific contact when GetAsync is called with contactId = 1
            var contact = new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 };
            _contactRepositoryMock.Setup(repo => repo.GetAsync(1)).ReturnsAsync(contact);

            // Act: Call the GetAsync method from the service to get the contact with the given contactId (1)
            var result = await _contactService.GetAsync(1);

            // Assert: Verify that the result is not null and the contact name is correct
            Xunit.Assert.NotNull(result); // Ensure that the result is not null
            Xunit.Assert.Equal("John Doe", result.ContactName); // Verify that the contact name matches the expected value
        }


        [Fact]
        public async Task GetAsync_InvalidContactId_ReturnsNull()
        {
            // Arrange: Set up the mock to return null when GetAsync is called with an invalid contactId (999)
            _contactRepositoryMock.Setup(repo => repo.GetAsync(999)).ReturnsAsync((Contact)null);

            // Act: Call the GetAsync method from the service with an invalid contactId (999)
            var result = await _contactService.GetAsync(999);

            // Assert: Verify that the result is null when the contactId is invalid
            Xunit.Assert.Null(result); // Ensure that the result is null, indicating no contact was found
        }


        [Fact]
        public async Task SaveAsync_ValidContact_ReturnsContactId()
        {
            // Arrange: Create a new Contact object and set up the mock repository to return 1 when SaveAsync is called.
            var contact = new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 };
            _contactRepositoryMock.Setup(repo => repo.SaveAsync(contact)).ReturnsAsync(1);

            // Act: Call the SaveAsync method from the service to save the contact
            var result = await _contactService.SaveAsync(contact);

            // Assert: Verify that the result is the expected ContactId (1), indicating the contact was saved successfully
            Xunit.Assert.Equal(1, result); // Ensure the result matches the ContactId returned by the mock
        }


        [Fact]
        public async Task DeleteAsync_ValidContactId_DeletesContact()
        {
            // Arrange: Set up a contactId (1) and mock the DeleteAsync method to simulate deleting a contact.
            int contactId = 1;
            _contactRepositoryMock.Setup(repo => repo.DeleteAsync(contactId)).Returns(Task.CompletedTask);

            // Act: Call the DeleteAsync method on the service to delete the contact.
            await _contactService.DeleteAsync(contactId);

            // Assert: Verify that the DeleteAsync method was called exactly once with the provided contactId.
            _contactRepositoryMock.Verify(repo => repo.DeleteAsync(contactId), Times.Once);
        }


        [Fact]
        public async Task GetContactsWithCompanyAndCountry_ReturnsListOfContacts()
        {
            // Arrange: Set up a list of contacts with associated CompanyId and CountryId
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 },
                new Contact { ContactId = 2, ContactName = "Jane Doe", CompanyId = 2, CountryId = 2 }
            };

            // Mock the GetContactsWithCompanyAndCountry method to return the contact list
            _specificContactRepositoryMock.Setup(repo => repo.GetContactsWithCompanyAndCountry()).ReturnsAsync(contacts);

            // Act: Call the GetContactsWithCompanyAndCountry method on the service
            var result = await _contactService.GetContactsWithCompanyAndCountry();

            // Assert: Verify that the result is not null and contains the expected number of contacts
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("John Doe", result[0].ContactName); // Verify the first contact's name
            Xunit.Assert.Equal("Jane Doe", result[1].ContactName); // Verify the second contact's name
        }


        [Fact]
        public async Task FilterContacts_ValidFilters_ReturnsFilteredContacts()
        {
            // Arrange: Create a list of contacts and specify filter criteria
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
            };
            int companyId = 1;
            int countryId = 1;

            // Mock the FilterContacts method to return the filtered contacts list based on the companyId and countryId
            _specificContactRepositoryMock.Setup(repo => repo.FilterContacts(countryId, companyId)).ReturnsAsync(contacts);

            // Act: Call the FilterContacts method on the service with valid filters
            var result = await _contactService.FilterContacts(countryId, companyId);

            // Assert: Verify that the result is not null and contains exactly one contact
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result); // Ensures only one contact is returned
            Xunit.Assert.Equal("John Doe", result[0].ContactName); // Verify that the contact's name matches "John Doe"
        }


        [Fact]
        public async Task FilterContacts_NoFilters_ReturnsAllContacts()
        {
            // Arrange: Create a list of contacts to return when no filters are applied
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 },
                new Contact { ContactId = 2, ContactName = "Jane Doe", CompanyId = 2, CountryId = 2 }
            };

            // Mock the FilterContacts method to return all contacts when no filters (null) are provided
            _specificContactRepositoryMock.Setup(repo => repo.FilterContacts(null, null)).ReturnsAsync(contacts);

            // Act: Call the FilterContacts method with no filters (null)
            var result = await _contactService.FilterContacts(null, null);

            // Assert: Verify that the result is not null and contains two contacts
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count); // Verifying there are 2 contacts returned
            Xunit.Assert.Equal("John Doe", result[0].ContactName); // Verify first contact's name
            Xunit.Assert.Equal("Jane Doe", result[1].ContactName); // Verify second contact's name
        }

    }
}