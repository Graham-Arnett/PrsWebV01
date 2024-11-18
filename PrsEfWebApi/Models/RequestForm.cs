namespace PrsEfWebApi.Models
{
    public class RequestForm
    {
        public string Description { get; set; }
        public string Justification { get; set; }
        public int UserId { get; set; }
        public string DeliveryMode { get; set; }
        public DateOnly DateNeeded { get; set; }
    }
}
