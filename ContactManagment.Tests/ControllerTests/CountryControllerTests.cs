using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ContactManagement.Models.Entities;
using ContactManagement.API.Controllers.V1;
using ContactManagement.BLL.Abstract;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ContactManagement.BLL.Requests;
using Microsoft.Extensions.Logging;

namespace ContactManagment.Tests.ControllerTests
{

    public class CountryControllerTests
    {
        private readonly Mock<ICountryService> _countryServiceMock;
        private readonly Mock<IValidator<Country>> _countryValidatorMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<CountryController>> _countryLoggerMock;
        private readonly CountryController _countryController;

        public CountryControllerTests()
        {
            _countryServiceMock = new Mock<ICountryService>();
            _countryValidatorMock = new Mock<IValidator<Country>>();
            _mediatorMock = new Mock<IMediator>();
            _countryLoggerMock = new Mock<ILogger<CountryController>>();
            _countryController = new CountryController(_countryServiceMock.Object, _countryValidatorMock.Object, _mediatorMock.Object, _countryLoggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsJsonResult()
        {
            // Arrange: Create a list of countries with a single entry
            var countries = new List<Country>
            {
                new Country { CountryId = 1, CountryName = "United States" }
            };

            // Mock the GetAllAsync method of the _countryServiceMock to return the list of countries
            _countryServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(countries);

            // Act: Call the GetAll method on the controller
            var result = await _countryController.GetAll();

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure the value of the JsonResult is not null
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Verify that the returned value is a List of Country objects
            var returnedCountries = jsonResult.Value as List<Country>;

            // Assert: Ensure the list contains exactly one country
            Xunit.Assert.Single(returnedCountries);

            // Assert: Verify that the country's name is "United States"
            Xunit.Assert.Equal("United States", returnedCountries[0].CountryName);
        }


        [Fact]
        public async Task Get_ValidCountryId_ReturnsCountry()
        {
            // Arrange: Create a country object to return from the service
            var country = new Country { CountryId = 1, CountryName = "United States" };

            // Mock the GetAsync method of the _countryServiceMock to return the country object for the valid countryId (1)
            _countryServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(country);

            // Act: Call the Get method on the controller with a valid countryId (1)
            var result = await _countryController.Get(1);

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure the value of the JsonResult is not null
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Verify that the returned value is a Country object
            var returnedCountry = jsonResult.Value as Country;

            // Assert: Check if the country's name is "United States"
            Xunit.Assert.Equal("United States", returnedCountry.CountryName);
        }


        [Fact]
        public async Task Get_InvalidCountryId_ReturnsInternalServerError()
        {
            // Arrange: Mock the GetAsync method to throw an exception when called with any integer (simulating an invalid countryId)
            _countryServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                              .ThrowsAsync(new Exception("Country not found"));

            // Act: Call the Get method on the controller with an invalid countryId (999)
            var result = await _countryController.Get(999);

            // Assert: Verify that the result is an ObjectResult (which is returned when an error occurs)
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);

            // Assert: Check if the status code is 500 (Internal Server Error)
            Xunit.Assert.Equal(500, statusCodeResult.StatusCode);
        }


        [Fact]
        public async Task Save_ValidCountry_ReturnsJsonResultWithCountryId()
        {
            // Arrange: Create a valid Country object
            var country = new Country { CountryId = 1, CountryName = "United States" };

            // Arrange: Create a validation result (empty, indicating no validation errors)
            var validationResult = new ValidationResult();

            // Arrange: Mock the validator to return the validation result when validating the country
            _countryValidatorMock.Setup(v => v.ValidateAsync(country, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Arrange: Mock the SaveAsync method to return the CountryId when saving the country
            _countryServiceMock.Setup(service => service.SaveAsync(country))
                   .ReturnsAsync(country.CountryId);

            // Act: Call the Save method on the controller
            var result = await _countryController.Save(country);

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Check if the "countryId" property is present in the result
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("countryId");
            Xunit.Assert.NotNull(propertyInfo);

            // Assert: Verify that the countryId returned matches the one passed to the Save method
            var countryId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(country.CountryId, countryId);
        }


        [Fact]
        public async Task Save_InvalidCountry_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange: Create an invalid Country object (CountryName is not set)
            var country = new Country();

            // Arrange: Create a list of validation errors (CountryName is required)
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("CountryName", "Country name is required")
            };

            // Arrange: Create a validation result using the validation errors
            var validationResult = new ValidationResult(validationErrors);

            // Arrange: Mock the validator to return the validation errors when validating the country
            _countryValidatorMock.Setup(v => v.ValidateAsync(country, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act: Call the Save method on the controller with the invalid country
            var result = await _countryController.Save(country);

            // Assert: Verify that the result is a BadRequestObjectResult
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);

            // Assert: Check that the BadRequestObjectResult contains a list of validation errors
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);

            // Assert: Ensure that there is exactly one validation error in the list
            Xunit.Assert.Single(errors);
        }


        [Fact]
        public async Task Delete_ValidCountryId_ReturnsOkResult()
        {
            // Arrange: Set the countryId to a valid value
            int countryId = 1;

            // Arrange: Mock the service to simulate the successful deletion of a country
            _countryServiceMock.Setup(service => service.DeleteAsync(countryId)).Returns(Task.CompletedTask);

            // Act: Call the Delete method on the controller with the valid countryId
            var result = await _countryController.Delete(countryId);

            // Assert: Verify that the result is an OkResult, indicating successful deletion
            Xunit.Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task GetCompanyStatisticsByCountryId_ValidCountryId_ReturnsJsonResult()
        {
            // Arrange: Create a dictionary to represent company statistics for the valid countryId
            var statistics = new Dictionary<string, int>
            {
                { "CompanyCount", 5 },
                { "ContactCount", 10 }
            };

            // Arrange: Mock the service to return the statistics when called with a valid countryId
            _countryServiceMock.Setup(service => service.GetCompanyStatisticsByCountryId(1)).ReturnsAsync(statistics);

            // Act: Call the GetCompanyStatisticsByCountryId method with the valid countryId
            var result = await _countryController.getCompanyStatisticsByCountryId(1);

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure the value in the JsonResult is not null
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Cast the value to a dictionary and verify the statistics are correct
            var returnedStatistics = jsonResult.Value as Dictionary<string, int>;

            // Assert: Check that the dictionary contains the expected number of entries
            Xunit.Assert.Equal(2, returnedStatistics.Count);

            // Assert: Verify that the "CompanyCount" is correct
            Xunit.Assert.Equal(5, returnedStatistics["CompanyCount"]);
        }


        [Fact]
        public async Task GetAllMediaTR_ReturnsJsonResult()
        {
            // Arrange: Create a list of countries to be returned by the mediator mock
            var countries = new List<Country>
            {
                new Country { CountryId = 1, CountryName = "United States" }
            };

            // Arrange: Mock the mediator to return the countries list when the GetAllCountriesQuery is sent
            _mediatorMock.Setup(mediator => mediator.Send(It.IsAny<GetAllCountriesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(countries);

            // Act: Call the GetAllMediatR method, which uses Mediator to get all countries
            var result = await _countryController.GetAllMediatR();

            // Assert: Verify that the result is a JsonResult
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);

            // Assert: Ensure the value in the JsonResult is not null
            Xunit.Assert.NotNull(jsonResult.Value);

            // Assert: Cast the value to a list of countries and verify that it contains the expected country
            var returnedCountries = jsonResult.Value as List<Country>;

            // Assert: Verify that the returned list contains only one country
            Xunit.Assert.Single(returnedCountries);

            // Assert: Check that the country name is "United States"
            Xunit.Assert.Equal("United States", returnedCountries[0].CountryName);
        }

    }
}
