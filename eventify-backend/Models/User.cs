using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePic { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? Road { get; set; }
        public string? City { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        public ICollection<Notification>? Notifications { get; set; }

    }
}
