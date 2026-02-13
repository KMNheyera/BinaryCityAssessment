using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;

namespace BC.ClientManager.BL.Repository.Interface
{
    public interface IClientRepository
    {
        Task<Client> CreateClientAsync(string name, CancellationToken ct = default);
        Task UpdateClientAsync(UpdateClientDto updateClientDto, CancellationToken ct = default);
        Task<IEnumerable<Client>> GetClientsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default);
        Task<Client?> GetClientByIdAsync(int clientId, CancellationToken ct = default);
        Task LinkContactAsync(int clientId, int contactId, CancellationToken ct = default);
        Task UnlinkContactAsync(int clientId, int contactId, CancellationToken ct = default);
        Task<IEnumerable<Contact>> GetContactsByClientAsync(int clientId, CancellationToken ct = default);
    }
}
