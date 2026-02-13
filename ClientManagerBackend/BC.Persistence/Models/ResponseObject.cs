namespace BC.Persistence.Models
{
    public class ResponseObject<T>
    {
        public T? Payload { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
    }

}
