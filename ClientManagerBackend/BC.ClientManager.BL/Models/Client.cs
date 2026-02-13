
namespace BC.ClientManager.BL.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ClientCode { get; set; }
        public int NumContacts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Optional convenience: display-friendly representation
        public override string ToString() => $"{ClientId}: {Name} ({ClientCode ?? "N/A"}) - Contacts: {NumContacts}";

        public static implicit operator Client(Task<Client> v)
        {
            throw new NotImplementedException();
        }
    }
}
