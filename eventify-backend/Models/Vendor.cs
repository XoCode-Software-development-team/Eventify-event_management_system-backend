using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class Vendor : User
    {
        public string? CompanyName { get; set; }
        public string? ContactPersonName { get; set; }

        public ICollection<ServiceAndResource>? ServiceAndResources { get; set; }
    }
}
