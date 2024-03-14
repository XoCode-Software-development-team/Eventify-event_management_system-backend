using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class Rating
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("ReviewAndRating")]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        public float Ratings { get; set; }

        public ReviewAndRating? ReviewAndRating { get; set; }
    }
}
