using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorSRPhoto
    {
        [Key, Column(Order = 0)]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; }

        [Key, Column(Order = 1)]
        public string? Image { get; set; }

        public ServiceAndResource? ServiceAndResource { get; set; }
    }
}
