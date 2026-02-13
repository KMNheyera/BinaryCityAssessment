using BC.ClientManager.BL.Dto;
using BC.ClientManager.BL.Models;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Settings;
using BC.Persistence.Helpers.Interface;
using Dapper;
using System.Data;

namespace BC.ClientManager.BL.Repository.Impementation
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDbContext _dbContext;

        public ContactRepository(IDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Contact> CreateContactAsync(CreateContactDto createContactDto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(createContactDto.Name)) throw new ArgumentException("Name is required", nameof(createContactDto.Name));
            if (string.IsNullOrWhiteSpace(createContactDto.Surname)) throw new ArgumentException("Surname is required", nameof(createContactDto.Surname));
            if (string.IsNullOrWhiteSpace(createContactDto.Email)) throw new ArgumentException("Email is required", nameof(createContactDto.Email));

            var dp = new DynamicParameters();
            dp.Add("@Name", createContactDto.Name);
            dp.Add("@Surname", createContactDto.Surname);
            dp.Add("@Email", createContactDto.Email);
            dp.Add("@OutContactId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbContext.ExecuteAsync(StoredProcedures.CreateContact, dp, commandType: CommandType.StoredProcedure, ct: ct).ConfigureAwait(false);

            var id = dp.Get<int>("@OutContactId");

            // Read back created contact for full details
            var contact = await GetContactByIdAsync(id, ct).ConfigureAwait(false);
            if (contact != null) return contact;

            return new Contact
            {
                ContactId = id,
                Name = createContactDto.Name,
                Surname = createContactDto.Surname,
                Email = createContactDto.Surname
            };
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(GetTableDataDto getTableDataDto, CancellationToken ct = default)
        {
            if (getTableDataDto.PageNumber <= 0) getTableDataDto.PageNumber = 1;
            if (getTableDataDto.PageSize < 0) getTableDataDto.PageSize = 0;

            var results = await _dbContext.QueryListAsync<Contact>(
                StoredProcedures.GetContacts,
                new { PageNumber = getTableDataDto.PageNumber, PageSize = getTableDataDto.PageSize, OrderBy = "Surname", OrderDir = "ASC" },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);

            return results;
        }

        public async Task<Contact?> GetContactByIdAsync(int contactId, CancellationToken ct = default)
        {
            if (contactId <= 0) throw new ArgumentException("Invalid contactId", nameof(contactId));

            var contact = await _dbContext.QuerySingleOrDefaultAsync<Contact>(
                StoredProcedures.GetContactById,
                new { ContactId = contactId },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);

            return contact;
        }

        public async Task<IEnumerable<Client>> GetClientsByContactAsync(int contactId, CancellationToken ct = default)
        {
            if (contactId <= 0) throw new ArgumentException("Invalid contactId", nameof(contactId));

            var results = await _dbContext.QueryListAsync<Client>(
                StoredProcedures.GetClientsByContact,
                new { ContactId = contactId },
                commandType: CommandType.StoredProcedure,
                ct: ct).ConfigureAwait(false);

            return results;
        }
    }
}

