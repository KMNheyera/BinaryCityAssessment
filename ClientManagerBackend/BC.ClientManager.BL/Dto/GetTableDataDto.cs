namespace BC.ClientManager.BL.Dto
{
    public class GetTableDataDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string? OrderBy { get; set; } = "Name";
        public string? OrderDir { get; set; } = "ASC";
    }
}
