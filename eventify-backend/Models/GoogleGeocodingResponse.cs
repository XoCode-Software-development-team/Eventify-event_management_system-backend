using Newtonsoft.Json;

namespace eventify_backend.Models
{
    public class GoogleGeocodingResponse
    {
        [JsonProperty("results")]
        public List<GeocodingResult>? Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class GeocodingResult
    {
        [JsonProperty("geometry")]
        public Geometry? Geometry { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("location")]
        public Location? Location { get; set; }
    }

    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

}
