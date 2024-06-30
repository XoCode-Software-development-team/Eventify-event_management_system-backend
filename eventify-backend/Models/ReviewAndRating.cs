using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ReviewAndRating
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Event")]
        public int EventId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; }

        [Key, Column(Order = 2)]
        public DateTime TimeSpan { get; set; }

        public float Ratings { get; set; }
        public string? Comment { get; set; }

        public Event? Event { get; set; }
        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
