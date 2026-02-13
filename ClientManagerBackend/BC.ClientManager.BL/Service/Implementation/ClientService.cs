using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Service.Interface;
using BC.Persistence.Models;
using Microsoft.Extensions.Logging;

namespace BC.ClientManager.BL.Service.Implementation
{
    public class ClientService : IClientService
    {

        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IClientRepository clientRepository,
            ILogger<ClientService> logger
            )
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        #region Public Members
        public async Task<ResponseObject<Client>> CreateClientAsync(string name, CancellationToken ct = default)
        {
            try
            {
                var result = await _clientRepository.CreateClientAsync(name, ct);
                return new ResponseObject<Client>
                {
                    Payload = result,
                    Message = $"Client with name {name} created successfully",
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
                _logger.LogError(ex, "Error creating client with name {Name}", name);
                return new ResponseObject<Client>
                {
                    Message = "Error creating client",
                    Success = false
                };
            }
        }

        public async Task<ResponseObject<Client?>> GetClientByIdAsync(int clientId, CancellationToken ct = default)
        {
            try
            {
                var result = await _clientRepository.GetClientByIdAsync(clientId, ct);
                if (result == null)
                {
                    return new ResponseObject<Client?>
                    {
                        Payload = null,
                        Message = $"Client with ID {clientId} not found",
                        Success = false
                    };
                }
                return new ResponseObject<Client?>
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
                _logger.LogError(ex, "Error retrieving client with ID {ClientId}", clientId);
                return new ResponseObject<Client?>
                {
                    Message = $"Error retrieving client with ID {clientId}",
                    Success = false
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<Client>>> GetClientsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default)
        {
            try
            {
                var result = await _clientRepository.GetClientsAsync(getTableDataDto, ct);
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
                _logger.LogError(ex, "Error retrieving clients");
                return new ResponseObject<IEnumerable<Client>>
                {
                    Message = "Error retrieving clients",
                    Success = false
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<Contact>>> GetContactsByClientAsync(int clientId, CancellationToken ct = default)
        {
            try
            {
                var result = await _clientRepository.GetContactsByClientAsync(clientId, ct);
                return new()
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
                _logger.LogError(ex, "Error retrieving contacts for client with ID {ClientId}", clientId);
                return new()
                {
                    Message = $"Error retrieving contacts for client with ID {clientId}",
                };
            }
        }

        public async Task<ResponseObject<string>> LinkContactAsync(int clientId, int contactId, CancellationToken ct = default)
        {
            try
            {
                await _clientRepository.LinkContactAsync(clientId, contactId, ct);
                return new ResponseObject<string>
                {
                    Message = "Contact linked successfully",
                    Success = true
                };

            }
            catch (ArgumentException ex)
            {
                return new()
                {
                    Message = ex.Message,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking contact with ID {ContactId} to client with ID {ClientId}", contactId, clientId);
                return new ResponseObject<string>
                {
                    Message = $"Error linking contact with ID {contactId} to client with ID {clientId}",
                };
            }
        }

        public async Task<ResponseObject<string>> UnlinkContactAsync(int clientId, int contactId, CancellationToken ct = default)
        {
            try
            {
                await _clientRepository.UnlinkContactAsync(clientId, contactId, ct);
                return new()
                {
                    Message = "Contact unlinked successfully",
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
                _logger.LogError(ex, "Error unlinking contact with ID {ContactId} to client with ID {ClientId}", contactId, clientId);
                return new()
                {
                    Message = $"Error inlinking contact with ID {contactId} to client with ID {clientId}"
                };
            }
        }

        public async Task<ResponseObject<string>> UpdateClientAsync(UpdateClientDto updateClientDto, CancellationToken ct = default)
        {
            try
            {
                await _clientRepository.UpdateClientAsync(updateClientDto, ct);
                return new()
                {
                    Message = "Client updated successfully",
                    Success = true
                };

            }
            catch (ArgumentException ex)
            {
                return new()
                {
                    Message = ex.Message,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client with ID {clientId}", updateClientDto.ClientId);
                return new()
                {
                    Message = $"Error updating client with ID {updateClientDto.ClientId}",
                };
            }
        }
        #endregion Public Members
    }
}
