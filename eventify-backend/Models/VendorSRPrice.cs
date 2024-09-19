using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorSRPrice
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; }  // Foreign key and part of composite primary key

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Price")]
        public int PId { get; set; }  // Foreign key and part of composite primary key

        public ICollection<Price>? Price { get; set; }  // Navigation property to Price

        public ServiceAndResource? ServiceAndResource { get; set; }  // Navigation property to ServiceAndResource
    }
}
