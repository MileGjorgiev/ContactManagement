using ContactManagement.BLL.Abstract;
using ContactManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Controllers.V1
{
    [Route("api/v1/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            List<Contact> contacts = await _contactService.GetAllAsync();
            return new JsonResult(contacts);
        }

        [HttpGet("{contactId}")]
        public async Task<IActionResult> Get(int contactId)
        {
            try
            {
                Contact contact = await _contactService.GetAsync(contactId: contactId);

                return new JsonResult(contact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("AddEditSummary", "Some fields are missing or contain invalid data. Please check your input and try again.");
                return BadRequest(ModelState);
            }

            await _contactService.SaveAsync(contact);

            return new JsonResult(new
            {
                contactId = contact.ContactId
            });
        }

        [HttpDelete("{contactId}")]
        public async Task<IActionResult> Delete(int contactId)
        {
            await _contactService.DeleteAsync(contactId);

            return Ok();
        }

        [HttpGet("/getContactsWithCompanyAndCountry")]
        public async Task<IActionResult> GetContactsWithCompanyAndCountry()
        {
            List<Contact> contacts = await _contactService.GetContactsWithCompanyAndCountry();
            return new JsonResult(contacts);
        }

        [HttpGet("/filterContacts")]
        public async Task<IActionResult> FilterContacts([FromQuery] int? companyId, [FromQuery] int? countryId)
        {
            List<Contact> contacts = await _contactService.FilterContacts(companyId,countryId);

            return new JsonResult(contacts);
        }
    }
}
