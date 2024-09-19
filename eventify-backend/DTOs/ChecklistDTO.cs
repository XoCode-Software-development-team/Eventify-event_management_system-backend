using eventify_backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.DTOs
{
    public class ChecklistDTO
    {
        public DateOnly Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<TaskDTO>? Tasks { get; set; }
    }
    public class TaskDTO
    {
        public bool Checked { get; set; }
        public string Time { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
    }
}
