using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorSRPhoto
    {
        [Key, Column(Order = 0)]
        [ForeignKey("ServiceAndResource")]
        public int Id { get; set; }  // Foreign key and part of composite primary key

        [Key, Column(Order = 1)]
        public int photoId { get; set; }
        public byte[]? Image { get; set; }  // Part of composite primary key

        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
