using System.ComponentModel.DataAnnotations;

namespace eventify_backend.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ProfilePic { get; set; } = string.Empty;
        public string HouseNo { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Road { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Notification>? Notifications { get; set; }

    }
}
