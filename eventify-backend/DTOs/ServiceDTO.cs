namespace eventify_backend.DTOs
{
    public class ServiceCategoryDTO
    {
        public int CategoryId { get; set; }
        public string? ServiceCategoryName { get; set; }
    }

    public class ServiceDTO // For admin view
    {
        public int SoRId { get; set; }
        public string? Service { get; set; }
        public float? Rating { get; set; }
        public bool IsSuspend { get; set; }
    }

    public class PriceModelDto
    {
        public int ModelId { get; set; }
        public string? ModelName { get; set; }
    }
}
