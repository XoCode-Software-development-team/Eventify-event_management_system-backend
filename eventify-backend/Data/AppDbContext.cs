using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ServiceAndResource> ServiceAndResources { get; set; }
        public DbSet<FeatureAndFacility> FeatureAndFacility { get; set; }

        public DbSet<Service> services { get; set; }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        public DbSet<VendorSRPhoto> VendorSRPhoto { get; set; }

        public DbSet<VendorSRVideo> VendorSRVideo { get; set; }

        public DbSet<Price> Prices { get; set; }
        public DbSet<PriceModel> PriceModels { get; set; }

        public DbSet<VendorSRPrice> VendorSRPrices { get; set; }

        public DbSet<VendorSRLocation> VendorSRLocation { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventSR> EventSr { get; set; }

        public DbSet<ReviewAndRating> ReviewAndRatings { get; set; }

        public DbSet<ReviewContent> ReviewContent { get; set; }

        public DbSet<Rating> Rating { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeatureAndFacility>()
                .HasKey(ff => new { ff.SoRId, ff.FacilityName });

            modelBuilder.Entity<FeatureAndFacility>()
                .HasOne(ff => ff.ServiceAndResource)
                .WithMany(sor => sor.FeaturesAndFacilities)
                .HasForeignKey(ff => ff.SoRId);

            modelBuilder.Entity<VendorSRPhoto>()
                .HasKey(vp => new { vp.Id, vp.photoId });

            modelBuilder.Entity<ServiceAndResource>()
                .HasMany(s => s.VendorRSPhotos)
                .WithOne(vr => vr.ServiceAndResource)
                .HasForeignKey(vr => vr.Id);

            modelBuilder.Entity<VendorSRVideo>()
                .HasKey(vv => new { vv.Id, vv.VideoId });

            modelBuilder.Entity<ServiceAndResource>()
                .HasMany(s => s.VendorRSVideos)
                .WithOne(vr => vr.ServiceAndResource)
                .HasForeignKey(vr => vr.Id);

            modelBuilder.Entity<VendorSRPrice>()
                .HasKey(vp => new { vp.ServiceAndResourceId, vp.PriceId });

            modelBuilder.Entity<VendorSRLocation>()
    .            HasKey(v => new { v.Id, v.LocationId });

            modelBuilder.Entity<VendorSRLocation>()
                .HasOne(v => v.ServiceAndResource)
                .WithMany(sr => sr.VendorSRLocations) // Specify the navigation property in ServiceAndResource entity
                .HasForeignKey(v => v.Id);

            modelBuilder.Entity<EventSR>()
                .HasKey(e => new { e.Id, e.SORId });

            modelBuilder.Entity<EventSR>()
                .HasOne(e => e.Event)
                .WithMany(e => e.EventSRs)
                .HasForeignKey(e => e.Id);

            modelBuilder.Entity<EventSR>()
                .HasOne(e => e.ServiceAndResource)
                .WithMany(e => e.EventSRs)
                .HasForeignKey(e => e.SORId);

            modelBuilder.Entity<ReviewContent>()
                .HasKey(r => new { r.Id, r.Content });

            modelBuilder.Entity<ReviewAndRating>()
                .HasMany(r => r.ReviewAndRatingContents)
                .WithOne(rc => rc.ReviewAndRating)
                .HasForeignKey(rc => rc.Id);

            modelBuilder.Entity<Rating>()
                .HasKey(r => new { r.Id, r.Ratings });

            modelBuilder.Entity<EventSoRApprove>()
              .HasKey(e => new { e.EventId, e.SoRId });
        }
    }

 
}
