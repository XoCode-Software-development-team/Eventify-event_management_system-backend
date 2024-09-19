using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class ServiceCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string ServiceCategoryName { get; set; } = string.Empty;


        public ICollection<Service>? Services { get; set; }
    }
}
