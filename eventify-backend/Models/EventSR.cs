using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class EventSR
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Event")]
        public int Id { get; set; }  // Foreign key referencing Event

        [Key, Column(Order = 1)]
        [ForeignKey("ServiceAndResource")]
        public int SORId { get; set; } // Foreign key referencing ServiceAndResource

        public Event? Event{ get; set; }  // Navigation property to Event
        public ServiceAndResource? ServiceAndResource { get; set; } // Navigation property to ServiceAndResource
    }
}
