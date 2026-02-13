namespace BC.ClientManager.BL.Dto
{
    public class UpdateClientDto
    {
        public int ClientId { get; set; }
        public string NewName { get; set; } = string.Empty;
        public bool RegenerateCode { get; set; } = false;
    }
}
