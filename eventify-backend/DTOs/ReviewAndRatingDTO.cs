using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventify_backend.DTOs
{
    public class ReviewAndRatingDTO
    {
        public int EventId { get; set; }
        public int SoRId { get; set; }
        public float Rate { get; set; }
        public string Review { get; set; } = string.Empty;
    }
}
