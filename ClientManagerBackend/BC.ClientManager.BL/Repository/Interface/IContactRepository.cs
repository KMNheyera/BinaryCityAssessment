using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;

namespace BC.ClientManager.BL.Repository.Interface
{
    public interface IContactRepository
    {
        Task<Contact> CreateContactAsync(CreateContactDto createContactDto, CancellationToken ct = default);
        Task<IEnumerable<Contact>> GetContactsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default);
        Task<Contact?> GetContactByIdAsync(int contactId, CancellationToken ct = default);
        Task<IEnumerable<Client>> GetClientsByContactAsync(int contactId, CancellationToken ct = default);
    }
}
