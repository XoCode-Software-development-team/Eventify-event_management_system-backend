using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Service : ServiceAndResource
    {
        public int Capacity { get; set; }

        [ForeignKey("ServiceCategory")]
        public int ServiceCategoryId { get; set; }

        public ServiceCategory? ServiceCategory { get; set; }

    }
}
