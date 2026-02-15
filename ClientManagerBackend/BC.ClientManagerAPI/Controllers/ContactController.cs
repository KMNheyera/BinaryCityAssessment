using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Service.Interface;
using BC.Persistence.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClientManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactController(
           IContactService contactService
            )
        {
            _contactService = contactService;
        }

        [HttpPost("create-contact")]
        public async Task<ResponseObject<Contact>> CreateClientAsync([FromBody] CreateContactDto request)
         => await _contactService.CreateContactAsync(request);

        [HttpGet("get-contacts")]
        public async Task<ResponseObject<IEnumerable<Contact>>> GetClientsAsync(
        [FromQuery] GetTableDataDto request)
            => await _contactService.GetContactsAsync(request);

        [HttpGet("get-contacts-by-id/{contactid}")]
        public async Task<ResponseObject<Contact>> GetContactByIdAsync(int contactid)
            => await _contactService.GetContactByIdAsync(contactid);

        [HttpGet("get-clients-by-contact/{contactid}")]
        public Task<ResponseObject<IEnumerable<Client>>> GetClientsByContactAsync(int contactid)
        => _contactService.GetClientsByContactAsync(contactid);
    }
}
