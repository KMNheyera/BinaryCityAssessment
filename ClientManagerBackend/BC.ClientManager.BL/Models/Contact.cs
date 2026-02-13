namespace BC.ClientManager.BL.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int NumClients { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Computed property for display/per requirements: "{Surname} {Name}"
        public string FullName => string.IsNullOrWhiteSpace(Surname) ? Name : $"{Surname} {Name}";

        public override string ToString() => $"{ContactId}: {FullName} <{Email}> - Clients: {NumClients}";
    }
}
