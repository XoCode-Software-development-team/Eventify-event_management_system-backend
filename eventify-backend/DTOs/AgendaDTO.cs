namespace eventify_backend.DTOs
{
    public class AgendaDTO
    {
        public DateOnly Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<TaskDTO>? Tasks { get; set; }
    }
}
