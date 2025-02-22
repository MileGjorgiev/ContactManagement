using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ContactManagement.Models.Entities;
using ContactManagement.API.Controllers.V1;
using ContactManagement.BLL.Abstract;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;


namespace ContactManagment.Tests.ControllerTests
{

    public class ContactControllerTests
    {
        private readonly Mock<IContactService> _contactServiceMock;
        private readonly Mock<IValidator<Contact>> _contactValidatorMock;
        private readonly Mock<ILogger<ContactController>> _contactLoggerMock;
        private readonly ContactController _contactController;

        public ContactControllerTests()
        {
            _contactServiceMock = new Mock<IContactService>();
            _contactValidatorMock = new Mock<IValidator<Contact>>();
            _contactLoggerMock = new Mock<ILogger<ContactController>>();
            _contactController = new ContactController(_contactServiceMock.Object, _contactLoggerMock.Object, _contactValidatorMock.Object);
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
            var contact = new Contact
            {
                ContactId = 1,
                ContactName = "John Doe", 
                CompanyId = 5,           // Ensure this ID exists in the database
                CountryId = 2            // Ensure this ID exists in the database
            };

            // Mock the validation to pass
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>()); 
            _contactValidatorMock.Setup(validator => validator.ValidateAsync(contact, It.IsAny<CancellationToken>()))
                                 .Returns(Task.FromResult(validationResult)); 


            _contactServiceMock.Setup(service => service.SaveAsync(contact))
                               .ReturnsAsync(contact.ContactId);

            // Act
            var result = await _contactController.Save(contact);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result); 
            Xunit.Assert.NotNull(jsonResult.Value);

           
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("contactId");
            Xunit.Assert.NotNull(propertyInfo);

            var contactId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(contact.ContactId, contactId);
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
