using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.Persistence.Models;

namespace BC.ClientManager.BL.Service.Interface
{
    public interface IContactService
    {
        Task<ResponseObject<Contact>> CreateContactAsync(CreateContactDto createContactDto, CancellationToken ct = default);
        Task<ResponseObject<IEnumerable<Contact>>> GetContactsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default);
        Task<ResponseObject<Contact>> GetContactByIdAsync(int contactId, CancellationToken ct = default);
        Task<ResponseObject<IEnumerable<Client>>> GetClientsByContactAsync(int contactId, CancellationToken ct = default);
    }
}
