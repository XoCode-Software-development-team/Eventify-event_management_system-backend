using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorSRLocation
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; } // Part of the composite key and foreign key to ServiceAndResource
        [Key]
        [Column(Order = 1)]
        public string? HouseNo { get; set; }
        [Key]
        [Column(Order = 2)]
        public string? Area { get; set; }
        [Key]
        [Column(Order = 3)]
        public string? District { get; set; }

        public string? Country { get; set; }

        public string? State { get; set; }

        public ServiceAndResource? ServiceAndResource { get; set; }
    }

}
