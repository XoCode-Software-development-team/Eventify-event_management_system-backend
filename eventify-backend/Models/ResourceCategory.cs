using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class ResourceCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string ResourceCategoryName { get; set; } = string.Empty;

        public ICollection<Resource>? Resources { get; set; }
    }
}
