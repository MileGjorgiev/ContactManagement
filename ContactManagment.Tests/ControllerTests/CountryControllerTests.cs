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
            // Arrange
            var countries = new List<Country>
        {
            new Country { CountryId = 1, CountryName = "United States" }
        };
            _countryServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(countries);

            // Act
            var result = await _countryController.GetAll();

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCountries = jsonResult.Value as List<Country>;
            Xunit.Assert.Single(returnedCountries);
            Xunit.Assert.Equal("United States", returnedCountries[0].CountryName);
        }

        [Fact]
        public async Task Get_ValidCountryId_ReturnsCountry()
        {
            // Arrange
            var country = new Country { CountryId = 1, CountryName = "United States" };
            _countryServiceMock.Setup(service => service.GetAsync(1)).ReturnsAsync(country);

            // Act
            var result = await _countryController.Get(1);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCountry = jsonResult.Value as Country;
            Xunit.Assert.Equal("United States", returnedCountry.CountryName);
        }

        [Fact]
        public async Task Get_InvalidCountryId_ReturnsInternalServerError()
        {
            // Arrange
            _countryServiceMock.Setup(service => service.GetAsync(It.IsAny<int>()))
                              .ThrowsAsync(new Exception("Country not found"));

            // Act
            var result = await _countryController.Get(999);

            // Assert
            var statusCodeResult = Xunit.Assert.IsType<ObjectResult>(result);
            Xunit.Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Save_ValidCountry_ReturnsJsonResultWithCountryId()
        {
            // Arrange
            var country = new Country { CountryId = 1, CountryName = "United States" };
            var validationResult = new ValidationResult(); // No errors, so it's valid

            _countryValidatorMock.Setup(v => v.ValidateAsync(country, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            _countryServiceMock.Setup(service => service.SaveAsync(country))
                   .ReturnsAsync(country.CountryId); // Return the CountryId as an int

            // Act
            var result = await _countryController.Save(country);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);

            // Use reflection to access the countryId property
            var value = jsonResult.Value;
            var propertyInfo = value.GetType().GetProperty("countryId");
            Xunit.Assert.NotNull(propertyInfo); // Ensure the property exists

            var countryId = (int)propertyInfo.GetValue(value);
            Xunit.Assert.Equal(country.CountryId, countryId);
        }

        [Fact]
        public async Task Save_InvalidCountry_ReturnsBadRequestWithValidationErrors()
        {
            // Arrange
            var country = new Country();
            var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("CountryName", "Country name is required")
        };
            var validationResult = new ValidationResult(validationErrors);

            _countryValidatorMock.Setup(v => v.ValidateAsync(country, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(validationResult);

            // Act
            var result = await _countryController.Save(country);

            // Assert
            var badRequestResult = Xunit.Assert.IsType<BadRequestObjectResult>(result);
            var errors = Xunit.Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
            Xunit.Assert.Single(errors);
        }

        [Fact]
        public async Task Delete_ValidCountryId_ReturnsOkResult()
        {
            // Arrange
            int countryId = 1;
            _countryServiceMock.Setup(service => service.DeleteAsync(countryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _countryController.Delete(countryId);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetCompanyStatisticsByCountryId_ValidCountryId_ReturnsJsonResult()
        {
            // Arrange
            var statistics = new Dictionary<string, int>
        {
            { "CompanyCount", 5 },
            { "ContactCount", 10 }
        };
            _countryServiceMock.Setup(service => service.GetContactsWithCompanyAndCountry(1)).ReturnsAsync(statistics);

            // Act
            var result = await _countryController.getCompanyStatisticsByCountryId(1);

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedStatistics = jsonResult.Value as Dictionary<string, int>;
            Xunit.Assert.Equal(2, returnedStatistics.Count);
            Xunit.Assert.Equal(5, returnedStatistics["CompanyCount"]);
        }

        [Fact]
        public async Task GetAllMediaTR_ReturnsJsonResult()
        {
            // Arrange
            var countries = new List<Country>
        {
            new Country { CountryId = 1, CountryName = "United States" }
        };
            _mediatorMock.Setup(mediator => mediator.Send(It.IsAny<GetAllCountriesQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(countries);

            // Act
            var result = await _countryController.GetAllMediaTR();

            // Assert
            var jsonResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.NotNull(jsonResult.Value);
            var returnedCountries = jsonResult.Value as List<Country>;
            Xunit.Assert.Single(returnedCountries);
            Xunit.Assert.Equal("United States", returnedCountries[0].CountryName);
        }
    }
}
