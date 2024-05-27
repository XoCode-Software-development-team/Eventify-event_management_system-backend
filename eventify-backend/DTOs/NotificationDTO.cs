namespace eventify_backend.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
        public bool Read { get; set; }
    }
}
