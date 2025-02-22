using ContactManagement.BLL.Abstract;
using ContactManagement.BLL.Concrete;
using ContactManagement.BLL.Validators;
using ContactManagement.Models.Entities;
using ContactManagement.Models.Pagination;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace ContactManagement.API.Controllers.V1
{
    [Route("api/v1/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IValidator<Company> _companyValidator;

        public CompanyController(ICompanyService companyService, IValidator<Company> companyValidator)
        {
            _companyService = companyService;
            _companyValidator = companyValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<Company> companies = await _companyService.GetAllAsync();
            return new JsonResult(companies);
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> Get(int companyId)
        {
            try
            {
                Company company = await _companyService.GetAsync(companyId: companyId);

                return new JsonResult(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Company company)
        {
            var validationResult = await _companyValidator.ValidateAsync(company);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _companyService.SaveAsync(company);

            return new JsonResult(new
            {
                companyId = company.CompanyId
            });
        }


        [HttpDelete("{companyId}")]
        public async Task<IActionResult> Delete(int companyId)
        {
            await _companyService.DeleteAsync(companyId);

            return Ok();
        }

        [HttpGet("/companyPagionation")]
        public async Task<ActionResult<PaginatedResponse<Company>>> GetAllPagination(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 2)
        {
            var companies = await _companyService.GetCompaniesWithPagination(pageNumber, pageSize);

            var totalCompanies = await _companyService.GetTotalCompanies(); 
            var totalPages = (int)Math.Ceiling(totalCompanies / (double)pageSize); 

            var response = new PaginatedResponse<Company>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalCompanies,
                Data = companies
            };

            return Ok(response);
        }
    }
}
