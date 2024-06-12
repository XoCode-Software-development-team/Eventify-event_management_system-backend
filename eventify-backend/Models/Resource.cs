using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Resource : ServiceAndResource
    {
        [ForeignKey("ResourceCategory")]

        public int ResourceCategoryId { get; set; }

        public ResourceCategory? ResourceCategory { get; set; }

        public ICollection<ResourceManual>? ResourceManual { get; set; }

    }
}
