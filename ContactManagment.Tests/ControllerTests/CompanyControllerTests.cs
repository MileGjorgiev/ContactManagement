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

        // Existing Tests
        [Fact]
        public async Task GetCompanies_ReturnsJsonResult()
        {
            // Arrange
            var companies = new List<Company> { new Company { CompanyId = 1, CompanyName = "Google" } };
            _companyServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(companies);

            // Act
            var result = await _companyController.GetAll();

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCompanies = jsonResult.Value as List<Company>;
            Xunit.Assert.Single(returnedCompanies);
            Xunit.Assert.Equal("Google", returnedCompanies[0].CompanyName);
        }

        [Fact]
        public async Task GetCompany_ReturnsCompany()
        {
            // Arrange
            var company = new Company { CompanyId = 1, CompanyName = "Apple" };
            _companyServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(company);

            // Act
            var result = await _companyController.Get(1);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCompany = jsonResult.Value as Company;
            Xunit.Assert.Equal("Apple", returnedCompany.CompanyName);
        }

        [Fact]
        public async Task Save_ValidCompany_ReturnsJsonResultWithCompanyId()
        {
            // Arrange
            var company = new Company { CompanyId = 8, CompanyName = "Google" }; // Ensure CompanyId is set
            var validationResult = new ValidationResult(); // No errors, so it's valid

            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            _companyServiceMock.Setup(s => s.SaveAsync(company))
                               .ReturnsAsync(company.CompanyId); // Return the CompanyId

            // Act
            var result = await _companyController.Save(company);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Debugging: Print the JsonResult content
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(jsonResult.Value);
            Console.WriteLine(jsonContent); // Inspect the output in the test console

            // Use reflection to access the companyId property
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("companyId");
            Xunit.Assert.NotNull(propertyInfo); // Ensure the property exists

            var companyId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(company.CompanyId, companyId); // Check the value
        }

        [Fact]
        public async Task Save_InvalidCompany_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var company = new Company();
            var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("CompanyName", "Name must be not be empty"),
            new ValidationFailure("CompanyName", "Name must be longer than 3 characters"),
        };
            var validationResult = new ValidationResult(validationErrors);

            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act
            var result = await _companyController.Save(company);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
            Xunit.Assert.Equal(2, errors.Count);
        }

        [Fact]
        public async Task DeleteCompany_ReturnsNoContent()
        {
            // Arrange
            int companyId = 1;
            _companyServiceMock.Setup(service => service.DeleteAsync(companyId)).Returns(Task.FromResult(true));

            // Act
            var result = await _companyController.Delete(companyId);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }

        // Additional Tests
        [Fact]
        public async Task GetCompany_InvalidCompanyId_ReturnsInternalServerError()
        {
            // Arrange
            _companyServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                              .ThrowsAsync(new Exception("Company not found"));

            // Act
            var result = await _companyController.Get(999);

            // Assert
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
            Xunit.Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Save_CompanyWithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var company = new Company { CompanyId = 1, CompanyName = "" }; // Empty name
            var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("CompanyName", "Company name is required"),
        };
            var validationResult = new ValidationResult(validationErrors);

            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act
            var result = await _companyController.Save(company);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
            Xunit.Assert.Equal(1, errors.Count);
        }

        [Fact]
        public async Task DeleteCompany_InvalidCompanyId_ReturnsOk()
        {
            // Arrange
            int companyId = 999; // Invalid ID
            _companyServiceMock.Setup(service => service.DeleteAsync(companyId)).Returns(Task.FromResult(false));

            // Act
            var result = await _companyController.Delete(companyId);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetAll_NoCompanies_ReturnsEmptyList()
        {
            // Arrange
            var companies = new List<Company>(); // Empty list
            _companyServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(companies);

            // Act
            var result = await _companyController.GetAll();

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCompanies = jsonResult.Value as List<Company>;
            Xunit.Assert.Empty(returnedCompanies);
        }

        [Fact]
        public async Task Save_CompanyWithNullName_ReturnsBadRequest()
        {
            // Arrange
            var company = new Company { CompanyId = 1, CompanyName = null }; // Null name
            var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("CompanyName", "Company name is required"),
        };
            var validationResult = new ValidationResult(validationErrors);

            _companyValidatorMock.Setup(v => v.ValidateAsync(company, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act
            var result = await _companyController.Save(company);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
            Xunit.Assert.Equal(1, errors.Count);
        }
    }

}