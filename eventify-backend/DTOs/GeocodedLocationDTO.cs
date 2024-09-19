namespace eventify_backend.DTOs
{
    public class GeocodedLocationDTO
    {

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string District { get; set; } = string.Empty;
        public int SoRId { get; set; }
    }
}
