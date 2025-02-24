using ContactManagement.BLL.Abstract;
using ContactManagement.BLL.Requests;
using ContactManagement.Models.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.API.Controllers.V1
{
    [Route("api/v1/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IValidator<Country> _countryValidator;
        private readonly ILogger<CountryController> _logger;
        private readonly IMediator _mediator;

        public CountryController(ICountryService countryService, IValidator<Country> countryValidator, IMediator mediator, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _countryValidator = countryValidator;
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of all countries.
        /// </summary>
        /// <returns>
        /// Returns a list of all countries in the system. 
        /// If an error occurs, returns a 500 Internal Server Error response.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<Country> countries = await _countryService.GetAllAsync();
                return new JsonResult(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching countries.");
            }
        }

        /// <summary>
        /// Retrieves a country by its ID.
        /// </summary>
        /// <param name="countryId">The ID of the country to retrieve.</param>
        /// <returns>
        /// Returns the country with the given ID. 
        /// If the country is not found, a 404 Not Found response is returned. 
        /// If an error occurs, a 500 Internal Server Error response is returned.
        /// </returns>
        [HttpGet("{countryId}")]
        public async Task<IActionResult> Get(int countryId)
        {
            try
            {
                Country country = await _countryService.GetAsync(countryId);

                if (country == null)
                {
                    _logger.LogWarning("Country with ID {CountryId} not found.", countryId);
                    return NotFound(new { error = $"Country with ID {countryId} not found." });
                }

                return new JsonResult(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching country with ID: {CountryId}", countryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Saves a new country or updates an existing one.
        /// </summary>
        /// <param name="country">The country object to be saved.</param>
        /// <returns>
        /// Returns a JSON object containing the saved country ID. 
        /// If validation fails, a 400 Bad Request response is returned. 
        /// If a database error occurs, a 500 Internal Server Error response is returned. 
        /// If the country is not found during the operation, a 404 Not Found response is returned.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Country country)
        {
            try
            {
                var validationResult = await _countryValidator.ValidateAsync(country);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                await _countryService.SaveAsync(country);

                return new JsonResult(new
                {
                    countryId = country.CountryId
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Country not found: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving country with ID: {CountryId}", country.CountryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "A database error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while saving country with ID: {CountryId}", country.CountryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Deletes a country by its ID.
        /// </summary>
        /// <param name="countryId">The ID of the country to be deleted.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response if the country is successfully deleted.
        /// If the country is not found, returns a 404 Not Found response with an error message.
        /// In case of an unexpected error, returns a 500 Internal Server Error with an error message.
        /// </returns>
        [HttpDelete("{countryId}")]
        public async Task<IActionResult> Delete(int countryId)
        {
            try
            {
                await _countryService.DeleteAsync(countryId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Country not found: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting country with ID: {CountryId}", countryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Retrieves company statistics for a given country based on the country ID.
        /// </summary>
        /// <param name="countryId">The ID of the country for which company statistics are to be fetched.</param>
        /// <returns>
        /// Returns a JSON object containing a dictionary of company names and their corresponding contact counts. 
        /// In case of an error, a 500 Internal Server Error response is returned.
        /// </returns>
        [HttpGet("/getCompanyStatisticsByCountryId")]
        public async Task<IActionResult> getCompanyStatisticsByCountryId([FromQuery] int countryId)
        {
            try
            {
                Dictionary<string, int> companies = await _countryService.GetCompanyStatisticsByCountryId(countryId);
                return new JsonResult(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching countries.");
            }
        }

        /// <summary>
        /// Retrieves all countries using MediatR.
        /// </summary>
        /// <returns>
        /// Returns a JSON object containing the list of all countries retrieved via a MediatR query.
        /// In case of an error, a 500 Internal Server Error response is returned.
        /// </returns>
        [HttpGet("/mediatR")]
        public async Task<IActionResult> GetAllMediatR()
        {
            try
            {
                var countries = await _mediator.Send(new GetAllCountriesQuery());
                return new JsonResult(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching countries.");
            }
        }
    }
}
