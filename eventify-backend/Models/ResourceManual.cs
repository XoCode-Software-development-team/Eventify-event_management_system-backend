using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ResourceManual
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("ServiceAndResource")]
        public int SoRId { get; set; }

        [Key][Column(Order = 1)]
        public string? Manual {  get; set; }

        public Resource? Resource { get; set; }
    }
}
