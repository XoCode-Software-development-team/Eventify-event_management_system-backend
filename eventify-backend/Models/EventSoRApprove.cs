using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class EventSoRApprove
    {
        [Key]
        [ForeignKey("Event")]
        public int EventId { get; set; }
        [Key]
        [ForeignKey("ServiceAndResource")]

        public int SoRId { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsApproved { get; set; }

        public Event? Event { get; set; }
        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
