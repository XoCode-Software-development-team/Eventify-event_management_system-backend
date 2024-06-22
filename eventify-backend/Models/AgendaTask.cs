using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class AgendaTask
    {
        [Key]
        public int AgendaTaskId { get; set; }
        public TimeOnly Time { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        [ForeignKey("Agenda")]
        public int AgendaId { get; set; }

        public Agenda? Agenda { get; set; }
    }
}
