using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Price
    {
        [Key]
        public int Pid { get; set; }
        public string? Pname { get; set; }
        public double BasePrice { get; set; }

        [ForeignKey("PriceModel")]
        public int ModelId { get; set; }

        public PriceModel? PriceModel { get; set; } // Corrected navigation property

        public ICollection<VendorSRPrice>? VendorSRPrices { get; set; }
    }
}
