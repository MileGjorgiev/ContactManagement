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
            // Arrange: Create a list of contacts and set up the mock service to return it
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
            };
            _contactServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(contacts);

            // Act: Call the GetAll method on the controller
            var result = await _contactController.GetAll();

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure that the value of the JsonResult is not null
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Check that the returned value is a list of contacts
            var returnedContacts = jsonResult.Value as List<Contact>;

            // Assert: Ensure that exactly one contact is returned
            Xunit.Assert.Single(returnedContacts);

            // Assert: Verify that the contact's name is "John Doe"
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }

        [Fact]
        public async Task Get_ValidContactId_ReturnsContact()
        {
            // Arrange: Create a contact and set up the mock service to return it for contactId 1
            var contact = new Contact { ContactId = 1, ContactName = "Jane Doe", CompanyId = 1, CountryId = 1 };
            _contactServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(contact);

            // Act: Call the Get method on the controller with contactId 1
            var result = await _contactController.Get(1);

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure that the value of the JsonResult is not null
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Verify that the returned value is a Contact object
            var returnedContact = jsonResult.Value as Contact;

            // Assert: Ensure that the contact's name is "Jane Doe"
            Xunit.Assert.Equal("Jane Doe", returnedContact.ContactName);
        }

        [Fact]
        public async Task Get_InvalidContactId_ReturnsInternalServerError()
        {
            // Arrange: Set up the mock service to throw an exception when calling GetAsync for any contactId
            _contactServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                              .ThrowsAsync(new Exception("Contact not found"));

            // Act: Call the Get method on the controller with an invalid contactId (999)
            var result = await _contactController.Get(999);

            // Assert: Verify that the result is an ObjectResult (since the exception should be handled and returned as an error)
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);

            // Assert: Check if the status code is 500 (Internal Server Error)
            Xunit.Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Save_ValidContact_ReturnsJsonResultWithContactId()
        {
            // Arrange: Create a valid contact object
            var contact = new Contact
            {
                ContactId = 1,
                ContactName = "John Doe",
                CompanyId = 5,
                CountryId = 2
            };

            // Arrange: Set up the validator mock to return a successful validation (no validation errors)
            var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>());
            _contactValidatorMock.Setup(validator => validator.ValidateAsync(contact, It.IsAny<CancellationToken>()))
                                 .Returns(Task.FromResult(validationResult));

            // Arrange: Set up the service mock to return the ContactId of the contact after save
            _contactServiceMock.Setup(service => service.SaveAsync(contact))
                               .ReturnsAsync(contact.ContactId);

            // Act: Call the Save method on the controller
            var result = await _contactController.Save(contact);

            // Assert: Ensure that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Ensure that the value of the JsonResult contains the "contactId" property
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("contactId");
            Xunit.Assert.NotNull(propertyInfo);

            // Assert: Ensure that the "contactId" in the result matches the contact's ContactId
            var contactId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(contact.ContactId, contactId);
        }


        [Fact]
        public async Task Delete_ValidContactId_ReturnsOkResult()
        {
            // Arrange: Set up the contactId to be deleted
            int contactId = 1;

            // Arrange: Set up the service mock to simulate successful deletion
            _contactServiceMock.Setup(service => service.DeleteAsync(contactId)).Returns(Task.CompletedTask);

            // Act: Call the Delete method on the controller
            var result = await _contactController.Delete(contactId);

            // Assert: Ensure that the result is an OkResult, indicating successful deletion
            Xunit.Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetContactsWithCompanyAndCountry_ReturnsJsonResult()
        {
            // Arrange: Create a list of contacts with associated company and country IDs
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
            };

            // Arrange: Set up the service mock to return the list of contacts when GetContactsWithCompanyAndCountry is called
            _contactServiceMock.Setup(service => service.GetContactsWithCompanyAndCountry()).ReturnsAsync(contacts);

            // Act: Call the GetContactsWithCompanyAndCountry method of the controller
            var result = await _contactController.GetContactsWithCompanyAndCountry();

            // Assert: Ensure the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure that the result contains a non-null value
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Ensure that the returned value is a list of contacts
            var returnedContacts = jsonResult.Value as List<Contact>;

            // Assert: Ensure that the list contains only one contact
            Xunit.Assert.Single(returnedContacts);

            // Assert: Ensure that the contact's name is "John Doe"
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }

        [Fact]
        public async Task FilterContacts_ValidFilters_ReturnsJsonResult()
        {
            // Arrange: Create a list of contacts that match the filter criteria
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
            };

            // Arrange: Set up the service mock to return the list of contacts when FilterContacts is called with CompanyId = 1 and CountryId = 1
            _contactServiceMock.Setup(service => service.FilterContacts(1, 1)).ReturnsAsync(contacts);

            // Act: Call the FilterContacts method of the controller with valid filter criteria
            var result = await _contactController.FilterContacts(1, 1);

            // Assert: Ensure the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure that the result contains a non-null value
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Ensure that the returned value is a list of contacts
            var returnedContacts = jsonResult.Value as List<Contact>;

            // Assert: Ensure that the list contains only one contact
            Xunit.Assert.Single(returnedContacts);

            // Assert: Ensure that the contact's name is "John Doe"
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }

        [Fact]
        public async Task FilterContacts_NoFilters_ReturnsJsonResult()
        {
            // Arrange: Create a list of contacts (the list will be returned when no filters are applied)
            var contacts = new List<Contact>
            {
                new Contact { ContactId = 1, ContactName = "John Doe", CompanyId = 1, CountryId = 1 }
            };

            // Arrange: Set up the service mock to return the list of contacts when FilterContacts is called with null filters
            _contactServiceMock.Setup(service => service.FilterContacts(null, null)).ReturnsAsync(contacts);

            // Act: Call the FilterContacts method of the controller without filters (null values)
            var result = await _contactController.FilterContacts(null, null);

            // Assert: Ensure the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure that the result contains a non-null value
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Ensure that the returned value is a list of contacts
            var returnedContacts = jsonResult.Value as List<Contact>;

            // Assert: Ensure that the list contains only one contact
            Xunit.Assert.Single(returnedContacts);

            // Assert: Ensure that the contact's name is "John Doe"
            Xunit.Assert.Equal("John Doe", returnedContacts[0].ContactName);
        }
    }
}
