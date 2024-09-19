namespace eventify_backend.DTOs
{
    public class ResourceCategoryDTO
    {
        public int CategoryId { get; set; }
        public string? ResourceCategoryName { get; set; }
    }

    public class ResourceDTO // For admin view
    {
        public int SoRId { get; set; }
        public string? Resource { get; set; }
        public float? Rating { get; set; }
        public bool IsSuspend { get; set; }
    }
}
