using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ReviewAndRating
    {
        [Key]
        [ForeignKey("Event")]
        public int EventId { get; set; }
        [Key]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; }
        public float Ratings { get; set; }
        public string? Comment { get; set; }
        public DateTime TimeSpan { get; set; }

        public Event? Event { get; set; }
        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
