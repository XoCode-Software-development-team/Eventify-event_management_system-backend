using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class PriceModel
    {
        [Key]
        public int ModelId { get; set; }
        public string? ModelName { get; set; }

        public Price? Price { get; set; } // Corrected navigation property
    }
}
