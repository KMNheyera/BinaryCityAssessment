using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.Persistence.Models;

namespace BC.ClientManager.BL.Service.Interface
{
    public interface IClientService
    {
        Task<ResponseObject<Client>> CreateClientAsync(string name, CancellationToken ct = default);
        Task<ResponseObject<string>> UpdateClientAsync(UpdateClientDto updateClientDto, CancellationToken ct = default);
        Task<ResponseObject<IEnumerable<Client>>> GetClientsAsync(GetTableDataDto getClientDto, CancellationToken ct = default);
        Task<ResponseObject<Client?>> GetClientByIdAsync(int clientId, CancellationToken ct = default);
        Task<ResponseObject<string>> LinkContactAsync(int clientId, int contactId, CancellationToken ct = default);
        Task<ResponseObject<string>> UnlinkContactAsync(int clientId, int contactId, CancellationToken ct = default);
        Task<ResponseObject<IEnumerable<Contact>>> GetContactsByClientAsync(int clientId, CancellationToken ct = default);
    }
}
