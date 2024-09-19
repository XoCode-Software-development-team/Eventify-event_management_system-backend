using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Agenda
    {
        [Key]
        public int AgendaId { get; set; }

        public DateOnly Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [ForeignKey("Event")]
        public int EventId { get; set; }

        public Event? Event { get; set; }

        public ICollection<AgendaTask>? AgendaTasks { get; set; }
    }
}
