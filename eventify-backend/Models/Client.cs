using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class Client : User
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<Event>? Events { get; set; }
    }
}
