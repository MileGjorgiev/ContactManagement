using ContactManagement.BLL.Abstract;
using ContactManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.Design;
using FluentValidation;
using ContactManagement.BLL.Concrete;
using ContactManagement.BLL.Validators;
using Microsoft.EntityFrameworkCore;

namespace ContactManagement.API.Controllers.V1
{
    [Route("api/v1/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IValidator<Contact> _contactValidator;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IContactService contactService, ILogger<ContactController> logger, IValidator<Contact> contactValidator)
        {
            _contactService = contactService;
            _logger = logger;
            _contactValidator = contactValidator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<Contact> contacts = await _contactService.GetAllAsync();
                return new JsonResult(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching contacts.");
            }
        }

        [HttpGet("{contactId}")]
        public async Task<IActionResult> Get(int contactId)
        {
            try
            {
                Contact contact = await _contactService.GetAsync(contactId: contactId);

                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {ContactId} not found.", contactId);
                    return NotFound(new { error = $"Contact with ID {contactId} not found." });
                }

                return new JsonResult(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching company with ID: {ContactId}", contactId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Contact contact)
        {
            try
            {
                var validationResult = await _contactValidator.ValidateAsync(contact);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                await _contactService.SaveAsync(contact);

                return new JsonResult(new { contactId = contact.ContactId });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "{Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving contact with ID: {ContactId}", contact.ContactId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "A database error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while saving contact with ID: {ContactId}", contact.ContactId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{contactId}")]
        public async Task<IActionResult> Delete(int contactId)
        {
            try
            {
                await _contactService.DeleteAsync(contactId);

                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Contact not found: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting contact with ID: {ContactId}", contactId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
            }
        }

        [HttpGet("/getContactsWithCompanyAndCountry")]
        public async Task<IActionResult> GetContactsWithCompanyAndCountry()
        {
            try
            {
                List<Contact> contacts = await _contactService.GetContactsWithCompanyAndCountry();
                return new JsonResult(contacts);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching contacts.");
            }
        }

        [HttpGet("/filterContacts")]
        public async Task<IActionResult> FilterContacts([FromQuery] int? companyId, [FromQuery] int? countryId)
        {
            try
            {
                List<Contact> contacts = await _contactService.FilterContacts(countryId, companyId );

                return new JsonResult(contacts);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "{Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while fetching contacts.");
            }
        }
    }
}
