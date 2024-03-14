using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ReviewAndRating
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Event")]
        public int EventId { get; set; }
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; }

        public DateTime TimeSpan { get; set; }



        public Event? Event { get; set; }
        public ServiceAndResource? ServiceAndResource { get; set; }
        public ICollection<ReviewContent>? ReviewAndRatingContents { get; set; }

        public ICollection<ReviewContent>? ReviewContents { get; set; }

        public ICollection<Rating>? Ratings { get; set; }

        public ICollection<EventSoRApprove>? EventSoRApprove { get; set; }
    }
}
