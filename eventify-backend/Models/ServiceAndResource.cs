using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class ServiceAndResource
    {
        [Key]
        public int SoRId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsSuspend { get; set; }
        public bool IsRequestToDelete { get; set; }

        public float? OverallRate { get; set; }

        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }

        public ICollection<FeatureAndFacility>? FeaturesAndFacilities { get; set; }

        public ICollection<VendorSRPhoto>? VendorRSPhotos { get; set; }

        public ICollection<VendorSRVideo>? VendorRSVideos { get; set; }

        public ICollection<VendorSRPrice>? VendorSRPrices { get; set; }

        public ICollection<VendorSRLocation>? VendorSRLocations { get; set; }

        public ICollection<EventSR>? EventSRs { get; set; }

        public ICollection<ReviewAndRating>? ReviewAndRating { get; set; }

        public Vendor? Vendor { get; set; }
    }
}
