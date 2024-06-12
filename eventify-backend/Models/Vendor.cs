using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class Vendor : User
    {
        public string CompanyName { get; set; } = string.Empty;
        public string ContactPersonName { get; set; } = string.Empty;

        public ICollection<ServiceAndResource>? ServiceAndResources { get; set; }
        public ICollection<VendorFollow>? VendorFollows { get; set; }

    }
}
