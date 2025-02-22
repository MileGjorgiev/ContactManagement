using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ContactManagement.Models.Entities;
using ContactManagement.API.Controllers.V1;
using ContactManagement.BLL.Abstract;


namespace ContactManagment.Tests.ControllerTests
{

    public class ContactControllerTests
    {
        private readonly Mock<IContactService> _contactServiceMock;
        private readonly ContactController _contactController;

        public ContactControllerTests()
        {
            _contactServiceMock = new Mock<IContactService>();
            _contactController = new ContactController(_contactServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult()
        {
            // Arrange
            var contacts = new List<Contact>
        {
            new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
        };
            _contactServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(contacts);

            // Act
            var result = await _contactController.GetAll();

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedContacts = jsonResult.Value as List<Contact>;
            Xunit.Assert.Single(returnedContacts);
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }

        [Fact]
        public async Task Get_ValidContactId_ReturnsContact()
        {
            // Arrange
            var contact = new Contact { ContactId = 1, ContactName = "Jane Doe", CompanyId = 1, CountryId = 1 };
            _contactServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(contact);

            // Act
            var result = await _contactController.Get(1);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedContact = jsonResult.Value as Contact;
            Xunit.Assert.Equal("Jane Doe", returnedContact.ContactName);
        }

        [Fact]
        public async Task Get_InvalidContactId_ReturnsInternalServerError()
        {
            // Arrange
            _contactServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                              .ThrowsAsync(new Exception("Contact not found"));

            // Act
            var result = await _contactController.Get(999);

            // Assert
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
            Xunit.Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Save_ValidContact_ReturnsJsonResultWithContactId()
        {
            // Arrange
            var contact = new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 };
            _contactServiceMock.Setup(service => service.SaveAsync(contact))
                   .ReturnsAsync(contact.ContactId); // Return the ContactId as an int

            // Act
            var result = await _contactController.Save(contact);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);

            // Use reflection to access the contactId property
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("contactId");
            Xunit.Assert.NotNull(propertyInfo); // Ensure the property exists

            var contactId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(contact.ContactId, contactId);
        }

        [Fact]
        public async Task Save_InvalidContact_ReturnsBadRequest()
        {
            // Arrange
            var contact = new Contact();
            _contactController.ModelState.AddModelError("ContactName", "Contact name is required");

            // Act
            var result = await _contactController.Save(contact);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            Xunit.Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ValidContactId_ReturnsOkResult()
        {
            // Arrange
            int contactId = 1;
            _contactServiceMock.Setup(service => service.DeleteAsync(contactId)).Returns(Task.CompletedTask);

            // Act
            var result = await _contactController.Delete(contactId);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetContactsWithCompanyAndCountry_ReturnsJsonResult()
        {
            // Arrange
            var contacts = new List<Contact>
        {
            new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
        };
            _contactServiceMock.Setup(service => service.GetContactsWithCompanyAndCountry()).ReturnsAsync(contacts);

            // Act
            var result = await _contactController.GetContactsWithCompanyAndCountry();

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedContacts = jsonResult.Value as List<Contact>;
            Xunit.Assert.Single(returnedContacts);
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }

        [Fact]
        public async Task FilterContacts_ValidFilters_ReturnsJsonResult()
        {
            // Arrange
            var contacts = new List<Contact>
        {
            new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
        };
            _contactServiceMock.Setup(service => service.FilterContacts(1, 1)).ReturnsAsync(contacts);

            // Act
            var result = await _contactController.FilterContacts(1, 1);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedContacts = jsonResult.Value as List<Contact>;
            Xunit.Assert.Single(returnedContacts);
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }

        [Fact]
        public async Task FilterContacts_NoFilters_ReturnsJsonResult()
        {
            // Arrange
            var contacts = new List<Contact>
        {
            new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
        };
            _contactServiceMock.Setup(service => service.FilterContacts(null, null)).ReturnsAsync(contacts);

            // Act
            var result = await _contactController.FilterContacts(null, null);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedContacts = jsonResult.Value as List<Contact>;
            Xunit.Assert.Single(returnedContacts);
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }
    }
}
