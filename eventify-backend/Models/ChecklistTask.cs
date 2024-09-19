using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ChecklistTask
    {
        [Key]
        public int ChecklistTaskId { get; set; }
        public bool Checked { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        [ForeignKey("Checklist")]
        public int ChecklistId { get; set; }

        public Checklist? Checklist { get; set; }

    }
}
