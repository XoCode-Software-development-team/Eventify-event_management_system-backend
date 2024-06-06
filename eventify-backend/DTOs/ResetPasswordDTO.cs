namespace eventify_backend.DTOs
{
    public record ResetPaswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string EmailToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
