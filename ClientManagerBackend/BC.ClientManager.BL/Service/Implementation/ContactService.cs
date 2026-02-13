using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Service.Interface;
using BC.Persistence.Models;
using Microsoft.Extensions.Logging;

namespace BC.ClientManager.BL.Service.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IContactRepository contactRepository,
            ILogger<ContactService> logger
            )
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task<ResponseObject<Contact>> CreateContactAsync(CreateContactDto createContactDto, CancellationToken ct = default)
        {
            try
            {
                var result = await _contactRepository.CreateContactAsync(createContactDto, ct);
                if (result == null)
                    return new ResponseObject<Contact>
                    {
                        Payload = null,
                        Message = "Failed to create contact.",
                        Success = false
                    };


                return new ResponseObject<Contact>
                {
                    Payload = result,
                    Message = "Contact created successfully.",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client with name {Name}", createContactDto.Name);
                return new ResponseObject<Contact>
                {
                    Message = "Error creating client with name",
                    Success = false
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<Client>>> GetClientsByContactAsync(int contactId, CancellationToken ct = default)
        {
            try
            {
                var result = await _contactRepository.GetClientsByContactAsync(contactId, ct);

                return new ResponseObject<IEnumerable<Client>>
                {
                    Payload = result,
                    Success = true
                }
                ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get clients with Contactid {contactId}", contactId);
                return new ResponseObject<IEnumerable<Client>>
                {
                    Message = $"Failed to get clients with Contactid {contactId}",
                    Success = false
                };
            }
        }

        public async Task<ResponseObject<Contact>> GetContactByIdAsync(int contactId, CancellationToken ct = default)
        {
            try
            {
                var result = await _contactRepository.GetContactByIdAsync(contactId, ct);

                return new ResponseObject<Contact>
                {
                    Payload = result,
                    Success = true
                }
                ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contacts by ID {contactId}", contactId);
                return new ResponseObject<Contact>
                {
                    Message = $"Failed to get contacts by ID {contactId}",
                    Success = false
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<Contact>>> GetContactsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default)
        {
            try
            {
                var result = await _contactRepository.GetContactsAsync(getTableDataDto, ct);
                return new ResponseObject<IEnumerable<Contact>>
                {
                    Payload = result,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contacts");
                return new ResponseObject<IEnumerable<Contact>>
                {
                    Message = "Failed to get contacts",
                    Success = false
                };
            }
        }
    }
}
