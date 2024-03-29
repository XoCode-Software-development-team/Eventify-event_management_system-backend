using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorSRVideo
    {
        [Key, Column(Order = 0)]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; } // Foreign key and part of composite primary key

        [Key, Column(Order = 1)]
        public string? Video { get; set; }  // Part of composite primary key

        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
