using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Service.Interface;
using BC.Persistence.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("create-client")]
        public async Task<ResponseObject<Client>> CreateClientAsync([FromBody] CreateClientDto createClientDto)
          => await _clientService.CreateClientAsync(createClientDto.ClientName);

        [HttpGet("get-client-by-id/{id}")]
        public async Task<ResponseObject<Client?>> GetClientById(int id)
          => await _clientService.GetClientByIdAsync(id);

        [HttpGet("get-clients")]
        public async Task<ResponseObject<IEnumerable<Client>>> GetClientsAsync(
        [FromQuery] GetTableDataDto request)
            => await _clientService.GetClientsAsync(request);

        [HttpGet("get-contacts-by-clientid/{clientid}")]
        public Task<ResponseObject<IEnumerable<Contact>>> GetContactsByClientAsync(int clientid)
         => _clientService.GetContactsByClientAsync(clientid);

        [HttpPost("update-client")]
        public Task<ResponseObject<string>> UpdateClientAsync([FromBody] UpdateClientDto request)
         => _clientService.UpdateClientAsync(request);

        [HttpPost("link-contact")]
        public Task<ResponseObject<string>> LinkContactAsync([FromBody] LinkContactDto request)
         => _clientService.LinkContactAsync(request.ClientId, request.ContactId);

        [HttpPost("unlink-contact")]
        public Task<ResponseObject<string>> UnlinkContactAsync([FromBody] LinkContactDto request)
         => _clientService.UnlinkContactAsync(request.ClientId, request.ContactId);
    }
}
