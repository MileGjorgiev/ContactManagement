using ContactManagement.BLL.Abstract;
using ContactManagement.BLL.Concrete;
using ContactManagement.BLL.Requests;
using ContactManagement.BLL.Validators;
using ContactManagement.Models.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Controllers.V1
{
    [Route("api/v1/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IValidator<Country> _countryValidator;
        private readonly IMediator _mediator;


        public CountryController(ICountryService countryService, IValidator<Country> countryValidator,IMediator mediator)
        {
            _countryService = countryService;
            _countryValidator = countryValidator;
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<Country> countries = await _countryService.GetAllAsync();
            return new JsonResult(countries);
        }

        [HttpGet("{countryId}")]
        public async Task<IActionResult> Get(int countryId)
        {
            try
            {
                Country country = await _countryService.GetAsync(countryId: countryId);

                return new JsonResult(country);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Country country)
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

        [HttpDelete("{countryId}")]
        public async Task<IActionResult> Delete(int countryId)
        {
            await _countryService.DeleteAsync(countryId);

            return Ok();
        }

        [HttpGet("/getCompanyStatisticsByCountryId")]
        public async Task<IActionResult> getCompanyStatisticsByCountryId([FromQuery] int countryId)
        {
            Dictionary<string, int> companies = await _countryService.GetContactsWithCompanyAndCountry(countryId);
            return new JsonResult(companies);
        }

        [HttpGet("/mediaTR")]
        public async Task<IActionResult> GetAllMediaTR()
        {
            var countries = await _mediator.Send(new GetAllCountriesQuery());
            return new JsonResult(countries);
        }
    }
}
