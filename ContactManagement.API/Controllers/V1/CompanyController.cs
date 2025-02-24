using ContactManagement.BLL.Abstract;
using ContactManagement.Models.Entities;
using ContactManagement.Models.Pagination;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.API.Controllers.V1
{
    [Route("api/v1/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IValidator<Company> _companyValidator;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompanyService companyService, IValidator<Company> companyValidator, ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _companyValidator = companyValidator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of all companies.
        /// </summary>
        /// <returns>
        /// Returns a JSON array containing all the companies. If an error occurs, 
        /// returns a 500 Internal Server Error response with a descriptive error message.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<Company> companies = await _companyService.GetAllAsync();
                return new JsonResult(companies);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching companies.");
            }
        }

        /// <summary>
        /// Retrieves a specific company by its ID.
        /// </summary>
        /// <param name="companyId">The ID of the company to retrieve.</param>
        /// <returns>
        /// Returns a JSON object containing the company details if found.
        /// If the company is not found, returns a 404 Not Found response.
        /// In case of an unexpected error, returns a 500 Internal Server Error response.
        /// </returns>
        [HttpGet("{companyId}")]
        public async Task<IActionResult> Get(int companyId)
        {
            try
            {
                Company company = await _companyService.GetAsync(companyId);
                if (company == null)
                {
                    _logger.LogWarning("Company with ID {CompanyId} not found.", companyId);
                    return NotFound(new { error = $"Company with ID {companyId} not found." });
                }
                return new JsonResult(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching company with ID: {CompanyId}", companyId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }


        /// <summary>
        /// Saves a new company or updates an existing company.
        /// </summary>
        /// <param name="company">The company object containing the details to be saved.</param>
        /// <returns>
        /// Returns a JSON object with the company ID if the operation is successful.
        /// If the country is not found during the operation, a 404 Not Found response is returned.
        /// If the company is invalid or there are errors, returns a BadRequest response with validation errors.
        /// In case of a database error, returns a 500 Internal Server Error with a database error message.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Company company)
        {
            try
            {
                var validationResult = await _companyValidator.ValidateAsync(company);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                await _companyService.SaveAsync(company);

                return new JsonResult(new { companyId = company.CompanyId });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Company not found: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving company with ID: {CompanyId}", company.CompanyId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "A database error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while saving company with ID: {CompanyId}", company.CompanyId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Deletes a company by its ID.
        /// </summary>
        /// <param name="companyId">The ID of the company to be deleted.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response if the company is successfully deleted.
        /// If the company is not found, returns a 404 Not Found response with an error message.
        /// In case of an unexpected error, returns a 500 Internal Server Error with an error message.
        /// </returns>
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> Delete(int companyId)
        {
            try
            {
                await _companyService.DeleteAsync(companyId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Company not found: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting company with ID: {CompanyId}", companyId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Retrieves a paginated list of companies.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (defaults to 1).</param>
        /// <param name="pageSize">The number of companies per page (defaults to 2).</param>
        /// <returns>
        /// Returns a paginated response containing the companies for the specified page, 
        /// the total number of pages, and the total number of records. 
        /// If invalid pagination parameters are provided, returns a 400 BadRequest response. 
        /// In case of a database error or any unexpected error, returns a 500 Internal Server Error.
        /// </returns>
        [HttpGet("/companyPagionation")]
        public async Task<ActionResult<PaginatedResponse<Company>>> GetAllPagination(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 2)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    _logger.LogWarning("Invalid pagination parameters. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid pagination parameters",
                        Detail = "PageNumber and PageSize must be greater than 0.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

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

                return new JsonResult(response);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while fetching companies.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Database error",
                    Detail = "An error occurred while accessing the database.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching companies with pagination.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Detail = "Please try again later.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
