using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Service.Interface;
using BC.Persistence.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

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

        #region Public Member
        public async Task<ResponseObject<Contact>> CreateContactAsync(CreateContactDto createContactDto, CancellationToken ct = default)
        {
            try
            {
                bool validEmail = IsValidEmailRegex(createContactDto.Email, out string msg);

                if (!validEmail)
                    return new()
                    {
                        Message = msg
                    };

                var result = await _contactRepository.CreateContactAsync(createContactDto, ct);
                if (result == null)
                    return new ResponseObject<Contact>
                    {
                        Message = "Failed to create contact."
                    };


                return new ResponseObject<Contact>
                {
                    Payload = result,
                    Message = "Contact created successfully.",
                    Success = true
                };
            }
            catch (ArgumentException ex)
            {
                return new()
                {
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client with name {Name}", createContactDto.Name);
                return new ResponseObject<Contact>
                {
                    Message = ex.Message
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
                };
            }
            catch (ArgumentException ex)
            {
                return new()
                {
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get clients with Contactid {contactId}", contactId);
                return new ResponseObject<IEnumerable<Client>>
                {
                    Message = $"Failed to get clients with Contactid {contactId}"
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
                };
            }
            catch (ArgumentException ex)
            {
                return new()
                {
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contacts by ID {contactId}", contactId);
                return new ResponseObject<Contact>
                {
                    Message = $"Failed to get contacts by ID {contactId}",
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
                    Message = "Failed to get contacts"
                };
            }
        }
        #endregion Public Member

        #region Private Member
        bool IsValidEmailRegex(string email, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                message = "Email cannot be empty.";
                return false;
            }

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            {
                message = "Email format is invalid.";
                return false;
            }

            return true;
        }
        #endregion Private Member

    }
}
