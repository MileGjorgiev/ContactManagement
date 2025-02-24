using ContactManagement.BLL.Abstract;
using ContactManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
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

        /// <summary>
        /// Retrieves a list of all contacts, accessible only to authorized users.
        /// </summary>
        /// <returns>
        /// Returns a list of all contacts. 
        /// If an error occurs, returns a 500 Internal Server Error with an appropriate error message.
        /// </returns>
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

        /// <summary>
        /// Retrieves a contact by its ID.
        /// </summary>
        /// <param name="contactId">The ID of the contact to retrieve.</param>
        /// <returns>
        /// Returns the contact with the specified ID.
        /// If the contact is not found, returns a 404 Not Found response with an error message.
        /// In case of an unexpected error, returns a 500 Internal Server Error with an error message.
        /// </returns>
        [HttpGet("{contactId}")]
        public async Task<IActionResult> Get(int contactId)
        {
            try
            {
                Contact contact = await _contactService.GetAsync(contactId);

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

        /// <summary>
        /// Saves a new contact or updates an existing one.
        /// </summary>
        /// <param name="contact">The contact object to save or update.</param>
        /// <returns>
        /// Returns the ID of the saved contact.
        /// If validation fails, returns a 400 Bad Request with validation errors.
        /// If the country is not found during the operation, a 404 Not Found response is returned.
        /// In case of a database error, returns a 500 Internal Server Error with a database error message.
        /// For any other unexpected error, returns a 500 Internal Server Error with a generic error message.
        /// </returns>
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

        /// <summary>
        /// Deletes a contact by its ID.
        /// </summary>
        /// <param name="contactId">The ID of the contact to delete.</param>
        /// <returns>
        /// Returns a 200 OK if the contact was successfully deleted.
        /// If the contact is not found, returns a 404 Not Found with the error message.
        /// In case of an unexpected error, returns a 500 Internal Server Error with a generic error message.
        /// </returns>
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

        /// <summary>
        /// Retrieves a list of contacts, including their associated company and country information.
        /// </summary>
        /// <returns>
        /// Returns a list of contacts with their related company and country details.
        /// In case of an unexpected error, returns a 500 Internal Server Error with a generic error message.
        /// </returns>
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

        /// <summary>
        /// Filters the contacts based on optional company and/or country ID parameters.
        /// </summary>
        /// <param name="companyId">
        /// An optional parameter for filtering contacts by company ID.
        /// </param>
        /// <param name="countryId">
        /// An optional parameter for filtering contacts by country ID.
        /// </param>
        /// <returns>
        /// Returns a list of filtered contacts based on the provided company and/or country ID.
        /// In case of an error, returns an appropriate error message:
        /// 404 Not Found if no matching country or company are found.
        /// 500 Internal Server Error for any unexpected issues.
        /// </returns>
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
