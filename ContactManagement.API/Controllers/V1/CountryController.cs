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

        [HttpGet("{countryId}")]
        public async Task<IActionResult> Get(int countryId)
        {
            try
            {
                Country country = await _countryService.GetAsync(countryId: countryId);

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

        [HttpGet("/getCompanyStatisticsByCountryId")]
        public async Task<IActionResult> getCompanyStatisticsByCountryId([FromQuery] int countryId)
        {
            Dictionary<string, int> companies = await _countryService.GetContactsWithCompanyAndCountry(countryId);
            return new JsonResult(companies);
        }

        [HttpGet("/mediatR")]
        public async Task<IActionResult> GetAllMediaTR()
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
