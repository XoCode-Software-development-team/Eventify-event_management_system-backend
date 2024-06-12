using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class VendorFollow
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }
        [Key, Column(Order =1)]
        [ForeignKey("Client")]
        public Guid ClientId { get; set; }

        public Client? Client { get; set; }

        public Vendor? Vendor { get; set; }
    }
}
