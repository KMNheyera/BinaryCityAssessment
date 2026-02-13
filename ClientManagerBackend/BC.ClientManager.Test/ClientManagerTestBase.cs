using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Repository.Impementation;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Service.Implementation;
using BC.ClientManager.BL.Service.Interface;
using BC.ClientManager.Test.Configurations;
using BC.Persistence.Helpers.Implementation;
using BC.Persistence.Helpers.Interface;
using BC.Persistence.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace BC.ClientManager.Test
{
    public abstract class ClientManagerTestBase
    {
        private readonly IClientService _clientService;
        private readonly IContactService _contactService;
        public ClientManagerTestBase()
        {
            var dbConnection = PersistanceConfig.ResolveDBConnection();

            IDbContext dbContext = new DbContext(dbConnection);

            IClientRepository clientRepository = new ClientRepository(dbContext);
            IContactRepository contactRepository = new ContactRepository(dbContext);

            ILogger<ContactService> _contactServiceLogger = Substitute.For<ILogger<ContactService>>();
            ILogger<ClientService> _clientServiceLogger = Substitute.For<ILogger<ClientService>>();

            _clientService = new ClientService(clientRepository, _clientServiceLogger);
            _contactService = new ContactService(contactRepository, _contactServiceLogger);
        }

        // Client Service Methods

        protected async Task<ResponseObject<Client>> CreateClientAsync(string name, CancellationToken ct = default)
        {
            return await _clientService.CreateClientAsync(name, ct);
        }

        protected async Task<ResponseObject<string>> UpdateClientAsync(UpdateClientDto updateClientDto, CancellationToken ct = default)
        {
            return await _clientService.UpdateClientAsync(updateClientDto, ct);
        }

        protected async Task<ResponseObject<IEnumerable<Client>>> GetClientsAsync(GetTableDataDto getClientDto, CancellationToken ct = default)
        {
            return await _clientService.GetClientsAsync(getClientDto, ct);
        }

        protected async Task<ResponseObject<Client?>> GetClientByIdAsync(int clientId, CancellationToken ct = default)
        {
            return await _clientService.GetClientByIdAsync(clientId, ct);
        }

        protected async Task<ResponseObject<string>> LinkContactAsync(int clientId, int contactId, CancellationToken ct = default)
        {
            return await _clientService.LinkContactAsync(clientId, contactId, ct);
        }

        protected async Task<ResponseObject<string>> UnlinkContactAsync(int clientId, int contactId, CancellationToken ct = default)
        {
            return await _clientService.UnlinkContactAsync(clientId, contactId, ct);
        }

        protected async Task<ResponseObject<IEnumerable<Contact>>> GetContactsByClientAsync(int clientId, CancellationToken ct = default)
        {
            return await _clientService.GetContactsByClientAsync(clientId, ct);
        }

        //Contact Service Methods
        protected async Task<ResponseObject<Contact>> CreateContactAsync(CreateContactDto createContactDto, CancellationToken ct = default)
        {
            return await _contactService.CreateContactAsync(createContactDto, ct);
        }

        protected async Task<ResponseObject<IEnumerable<Contact>>> GetContactsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default)
        {
            return await _contactService.GetContactsAsync(getTableDataDto, ct);
        }

        protected async Task<ResponseObject<Contact>> GetContactByIdAsync(int contactId, CancellationToken ct = default)
        {
            return await _contactService.GetContactByIdAsync(contactId, ct);
        }

        protected async Task<ResponseObject<IEnumerable<Client>>> GetClientsByContactAsync(int contactId, CancellationToken ct = default)
        {
            return await _contactService.GetClientsByContactAsync(contactId, ct);
        }

    }

}
