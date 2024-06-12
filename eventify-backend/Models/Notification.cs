using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; }
        public bool Read {  get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public User? User { get; set; }

    }
}
