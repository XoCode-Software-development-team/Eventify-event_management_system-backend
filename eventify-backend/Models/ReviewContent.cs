using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ReviewContent
    {
        [Key]
        [ForeignKey("ReviewAndRating")]
        public int Id { get; set; }

        public string? Content { get; set; }

        public ReviewAndRating? ReviewAndRating { get; set; }
    }
}
