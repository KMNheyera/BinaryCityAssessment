using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Settings;
using BC.Persistence.Helpers.Interface;
using Dapper;
using System.Data;

namespace BC.ClientManager.BL.Repository.Impementation
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbContext _executor;

        public ClientRepository(IDbContext executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<Client> CreateClientAsync(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));

            var dp = new DynamicParameters();
            dp.Add("@Name", name);
            dp.Add("@OutClientId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            dp.Add("@OutClientCode", dbType: DbType.String, size: 6, direction: ParameterDirection.Output);

            await _executor.ExecuteAsync(StoredProcedures.CreateClient, dp, commandType: CommandType.StoredProcedure, ct: ct).ConfigureAwait(false);

            var id = dp.Get<int>("@OutClientId");
            var code = dp.Get<string>("@OutClientCode");

            // Read back the created client via stored proc/view for full data
            var client = await _executor.QuerySingleOrDefaultAsync<Client>(StoredProcedures.GetClientById, new { ClientId = id }, commandType: CommandType.StoredProcedure, ct: ct).ConfigureAwait(false);
            if (client != null) return client;

            // Fallback minimal object if not returned
            return new Client
            {
                ClientId = id,
                Name = name,
                ClientCode = code
            };
        }

        public async Task UpdateClientAsync(UpdateClientDto updateClientDto, CancellationToken ct = default)
        {
            if (updateClientDto.ClientId <= 0) throw new ArgumentException("Invalid clientId", nameof(updateClientDto.ClientId));
            if (string.IsNullOrWhiteSpace(updateClientDto.NewName)) throw new ArgumentException("newName is required", nameof(updateClientDto.NewName));

            await _executor.ExecuteAsync(
                "dbo.sp_UpdateClient",
                new { ClientId = updateClientDto.ClientId, NewName = updateClientDto.NewName, RegenerateCode = updateClientDto.RegenerateCode ? 1 : 0 },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Client>> GetClientsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default)
        {
            // Validate inputs
            if (getTableDataDto.PageNumber <= 0) getTableDataDto.PageNumber = 1;
            if (getTableDataDto.PageSize < 0) getTableDataDto.PageSize = 0;

            var results = await _executor.QueryListAsync<Client>(
                StoredProcedures.GetClients,
                new { PageNumber = getTableDataDto.PageNumber, PageSize = getTableDataDto.PageSize, OrderBy = "Name", OrderDir = "ASC" },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);

            return results;
        }

        public async Task<Client?> GetClientByIdAsync(int clientId, CancellationToken ct = default)
        {
            if (clientId <= 0) throw new ArgumentException("Invalid clientId", nameof(clientId));

            var client = await _executor.QuerySingleOrDefaultAsync<Client>(
                StoredProcedures.GetClientById,
                new { ClientId = clientId },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);

            return client;
        }

        public async Task LinkContactAsync(int clientId, int contactId, CancellationToken ct = default)
        {
            if (clientId <= 0) throw new ArgumentException("Invalid clientId", nameof(clientId));
            if (contactId <= 0) throw new ArgumentException("Invalid contactId", nameof(contactId));

            await _executor.ExecuteAsync(
                StoredProcedures.LinkContactToClient,
                new { ClientId = clientId, ContactId = contactId },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);
        }

        public async Task UnlinkContactAsync(int clientId, int contactId, CancellationToken ct = default)
        {
            if (clientId <= 0) throw new ArgumentException("Invalid clientId", nameof(clientId));
            if (contactId <= 0) throw new ArgumentException("Invalid contactId", nameof(contactId));

            await _executor.ExecuteAsync(
                StoredProcedures.UnlinkContactFromClient,
                new { ClientId = clientId, ContactId = contactId },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contact>> GetContactsByClientAsync(int clientId, CancellationToken ct = default)
        {
            if (clientId <= 0) throw new ArgumentException("Invalid clientId", nameof(clientId));

            // sp_GetContactsByClient returns ContactId, FullName (Surname + ' ' + Name), Email
            var rows = await _executor.QueryListAsync<dynamic>(
                StoredProcedures.GetContactsByClient,
                new { ClientId = clientId },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);

            var list = new List<Contact>();
            foreach (var r in rows)
            {
                // r.FullName expected like "Surname Name"
                string full = r.FullName ?? string.Empty;
                string surname = string.Empty;
                string name = string.Empty;
                var parts = full.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    surname = parts[0];
                    name = parts[1];
                }
                else if (parts.Length == 1)
                {
                    surname = parts[0];
                }

                list.Add(new Contact
                {
                    ContactId = r.ContactId,
                    Name = name,
                    Surname = surname,
                    Email = r.Email
                });
            }

            return list;
        }
    }
}