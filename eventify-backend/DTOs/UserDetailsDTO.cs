namespace eventify_backend.DTOs
{
    public class UserDetailsDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPersonName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string HouseNo { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Road { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
