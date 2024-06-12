using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string? Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int GuestCount { get; set; }
        public byte[]? Thumbnail { get; set; }

        /*
          [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public int GuestCount { get; set; }

        public string? Thumbnail { get; set; }
        */

        // Foreign key referencing the Client who created the event
        [ForeignKey("Client")]
        public Guid? ClientId { get; set; }

        // Navigation property to represent the Client who created the event
        public Client? Client { get; set; }

        public ICollection<EventSR>? EventSRs { get; set; }

        public ICollection<ReviewAndRating>? ReviewAndRating { get; set; }

        public ICollection<EventSoRApprove>? EventSoRApproves { get; set; }
    }
}
