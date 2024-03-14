using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorSRLocation
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("ServiceAndResource")]
        public int Id { get; set; } // Part of the composite key and foreign key to ServiceAndResource
        [Key]
        [Column(Order = 0)]
        public int LocationId { get; set; }

        public string? HouseNo { get; set; } // Part of the composite key

        public string? Area { get; set; } // Part of the composite key

        public string? District { get; set; } // Part of the composite key

        public string? Country { get; set; } // Part of the composite key

        public string? State { get; set; } // Part of the composite key

        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
