using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ContactManagement.Models.Entities;
using ContactManagement.API.Controllers.V1;
using ContactManagement.BLL.Abstract;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace ContactManagment.Tests.ControllerTests
{
    public class CompanyControllerTests
    {
        private readonly Mock<ICompanyService> _companyServiceMock;
        private readonly Mock<IValidator<Company>> _companyValidatorMock;
        private readonly Mock<ILogger<CompanyController>> _companyLoggerMock;
        private readonly CompanyController _companyController;

        public CompanyControllerTests()
        {
            _companyServiceMock = new Mock<ICompanyService>();
            _companyValidatorMock = new Mock<IValidator<Company>>();
            _companyLoggerMock = new Mock<ILogger<CompanyController>>();
            _companyController = new CompanyController(_companyServiceMock.Object, _companyValidatorMock.Object, _companyLoggerMock.Object);
        }


        [Fact]
        public async Task GetCompanies_ReturnsJsonResult()
        {
            // Arrange: Create a list of companies to be returned by the mock service
            var companies = new List<Company> { new Company { CompanyId = 1, CompanyName = "Google" } };

            // Arrange: Set up the mock service to return the above list of companies when GetAllAsync() is called
            _companyServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(companies);

            // Act: Call the GetAll method on the controller, which should invoke the mock service
            var result = await _companyController.GetAll();

            // Assert: Verify that the result is of type JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Check that the JsonResult has a non-null value
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Cast the JsonResult value to a list of companies and check if it's the expected list
            var returnedCompanies = jsonResult.Value as List<Company>;

            // Assert: Ensure only one company is returned
            Xunit.Assert.Single(returnedCompanies);

            // Assert: Verify that the company name matches the expected value ("Google")
            Xunit.Assert.Equal("Google", returnedCompanies[0].CompanyName);
        }

        [Fact]
        public async Task GetCompany_ReturnsCompany()
        {
            // Arrange: Create a company object to be returned by the mock service
            var company = new Company { CompanyId = 1, CompanyName = "Apple" };

            // Arrange: Set up the mock service to return the above company when GetAsync(1) is called
            _companyServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(company);

            // Act: Call the Get method on the controller, which should invoke the mock service
            var result = await _companyController.Get(1);

            // Assert: Verify that the result is of type JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure the JsonResult contains a non-null value
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Cast the JsonResult value to a company and check if it's the expected company
            var returnedCompany = jsonResult.Value as Company;

            // Assert: Verify that the returned company name matches the expected value ("Apple")
            Xunit.Assert.Equal("Apple", returnedCompany.CompanyName);
        }

        [Fact]
        public async Task Save_ValidCompany_ReturnsJsonResultWithCompanyId()
        {
            // Arrange: Create a valid company object to be saved
            var company = new Company { CompanyId = 8, CompanyName = "Google" };

            // Arrange: Set up the mock validator to simulate a successful validation result
            var validationResult = new ValidationResult(); // Simulating a valid company
            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Arrange: Set up the mock service to return the company ID when the SaveAsync method is called
            _companyServiceMock.Setup(s => s.SaveAsync(company))
                               .ReturnsAsync(company.CompanyId); // Simulating saving the company and returning its ID

            // Act: Call the Save method on the controller
            var result = await _companyController.Save(company);

            // Assert: Verify that the result is of type JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Serialize the result content and optionally print it for debugging
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(jsonResult.Value);
            Console.WriteLine(jsonContent);

            // Assert: Ensure that the JsonResult contains a property named "companyId"
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("companyId");
            Xunit.Assert.NotNull(propertyInfo);

            // Assert: Retrieve the "companyId" value and check that it matches the company ID
            var companyId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(company.CompanyId, companyId);
        }

        [Fact]
        public async Task Save_InvalidCompany_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange: Create an invalid company object (missing required properties)
            var company = new Company(); // CompanyName is required, but it's empty

            // Arrange: Set up the mock validator to simulate validation errors
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("CompanyName", "Name must not be empty"),
                new ValidationFailure("CompanyName", "Name must be longer than 3 characters"),
            };

            var validationResult = new ValidationResult(validationErrors);

            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult); // Simulating validation errors

            // Act: Call the Save method on the controller
            var result = await _companyController.Save(company);

            // Assert: Verify that the result is a BadRequestObjectResult
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);

            // Assert: Verify that the BadRequest response contains the validation errors
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);

            // Assert: Check that two validation errors are returned
            Xunit.Assert.Equal(2, errors.Count);
        }

        [Fact]
        public async Task DeleteCompany_ReturnsNoContent()
        {
            // Arrange: Set up a company ID to delete and mock the service method to return true
            int companyId = 1;
            _companyServiceMock.Setup(service => service.DeleteAsync(companyId)).Returns(Task.FromResult(true)); // Simulate successful deletion

            // Act: Call the Delete method on the controller
            var result = await _companyController.Delete(companyId);

            // Assert: Verify that the result is an OkResult (No Content)
            Xunit.Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task GetCompany_InvalidCompanyId_ReturnsInternalServerError()
        {
            // Arrange: Set up the service mock to throw an exception when trying to get a company by any ID
            _companyServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                               .ThrowsAsync(new Exception("Company not found")); // Simulate error for invalid company ID

            // Act: Call the Get method on the controller with an invalid company ID
            var result = await _companyController.Get(999); // 999 is used as a non-existing company ID

            // Assert: Verify that the result is an ObjectResult with a 500 (Internal Server Error) status code
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
            Xunit.Assert.Equal(500, statusCodeResult.StatusCode); // Expecting an Internal Server Error (500)
        }


        [Fact]
        public async Task Save_CompanyWithEmptyName_ReturnsBadRequest()
        {
            // Arrange: Create a company object with an empty name and a validation error
            var company = new Company { CompanyId = 1, CompanyName = "" }; // Invalid company name
            var validationErrors = new List<ValidationFailure>
    {
        new ValidationFailure("CompanyName", "Company name is required") // Validation failure for the company name
    };
            var validationResult = new ValidationResult(validationErrors);

            // Set up the mock validator to return the validation result for the company
            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act: Call the Save method on the controller
            var result = await _companyController.Save(company);

            // Assert: Verify that the result is a BadRequest with the correct validation error
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result); // Check if BadRequest is returned
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value); // Verify the errors in the response
            Xunit.Assert.Equal(1, errors.Count); // Check if only 1 validation error is returned
        }

        [Fact]
        public async Task DeleteCompany_InvalidCompanyId_ReturnsOk()
        {
            // Arrange: Set up an invalid company ID
            int companyId = 999;

            // Mock the DeleteAsync method to return false, indicating that no company was deleted
            _companyServiceMock.Setup(service => service.DeleteAsync(companyId)).Returns(Task.FromResult(false));

            // Act: Call the Delete method on the controller with the invalid ID
            var result = await _companyController.Delete(companyId);

            // Assert: Verify that the result is an OkResult, indicating that the operation was successful
            // even if the company wasn't found.
            Xunit.Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetAll_NoCompanies_ReturnsEmptyList()
        {
            // Arrange: Set up the mock to return an empty list of companies
            var companies = new List<Company>();
            _companyServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(companies);

            // Act: Call the GetAll method on the controller
            var result = await _companyController.GetAll();

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert that the value is not null and is an empty list
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCompanies = jsonResult.Value as List<Company>;

            // Assert: Ensure the returned list is empty
            Xunit.Assert.Empty(returnedCompanies);
        }

        [Fact]
        public async Task Save_CompanyWithNullName_ReturnsBadRequest()
        {
            // Arrange: Create a company with a null name and set up the validation to return an error
            var company = new Company { CompanyId = 1, CompanyName = null };
            
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("CompanyName", "Company name is required")
            };
            var validationResult = new ValidationResult(validationErrors);

            // Set up the validator mock to return the validation result
            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act: Call the Save method on the controller
            var result = await _companyController.Save(company);

            // Assert: Verify that the result is a BadRequestObjectResult
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);

            // Assert: Verify that the validation error is in the response and the count is correct
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
            Xunit.Assert.Equal(1, errors.Count);
        }
    }

}