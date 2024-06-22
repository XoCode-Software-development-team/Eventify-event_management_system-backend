using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Checklist
    {
        [Key]
        public int ChecklistId { get; set; }
        public DateOnly Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [ForeignKey("Event")]
        public int EventId { get; set; }
        
        public Event? Event { get; set; }

        public ICollection<ChecklistTask>? ChecklistTasks { get; set; }
    }
}
