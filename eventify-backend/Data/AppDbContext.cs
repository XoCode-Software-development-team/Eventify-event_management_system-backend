﻿using eventify_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace eventify_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ServiceAndResource> ServiceAndResources { get; set; }
        public DbSet<FeatureAndFacility> FeatureAndFacility { get; set; }

        public DbSet<Service> Services { get; set; }

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

        public DbSet<EventSoRApprove> EventSoRApproves { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<ResourceCategory> ResourceCategories { get; set; }

        public DbSet<ResourceManual> ResourceManual { get; set; }

        public DbSet<Notification> Notification { get; set; }

        public DbSet<VendorFollow> VendorFollows { get; set; }

        public DbSet<Agenda> Agenda { get; set; }
        public DbSet<Checklist> Checklist { get; set; }
        public DbSet<ChecklistTask> ChecklistTask { get; set; }
        public DbSet<AgendaTask> AgendaTask { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeatureAndFacility>()
                .HasKey(ff => new { ff.SoRId, ff.FacilityName });

            modelBuilder.Entity<FeatureAndFacility>()
                .HasOne(ff => ff.ServiceAndResource)
                .WithMany(sor => sor.FeaturesAndFacilities)
                .HasForeignKey(ff => ff.SoRId);

            modelBuilder.Entity<VendorSRPhoto>()
                .HasKey(vp => new { vp.SoRId, vp.Image });

            modelBuilder.Entity<ServiceAndResource>()
                .HasMany(s => s.VendorRSPhotos)
                .WithOne(vr => vr.ServiceAndResource)
                .HasForeignKey(vr => vr.SoRId);

            modelBuilder.Entity<VendorSRVideo>()
                .HasKey(vv => new { vv.SoRId, vv.Video });

            modelBuilder.Entity<ServiceAndResource>()
                .HasMany(s => s.VendorRSVideos)
                .WithOne(vr => vr.ServiceAndResource)
                .HasForeignKey(vr => vr.SoRId);

            modelBuilder.Entity<VendorSRPrice>()
                .HasKey(vp => new { vp.SoRId, vp.PId });

            modelBuilder.Entity<VendorSRLocation>()
    .            HasKey(v => new { v.SoRId, v.HouseNo, v.Area, v.District });

            modelBuilder.Entity<VendorSRLocation>()
                .HasOne(v => v.ServiceAndResource)
                .WithMany(sr => sr.VendorSRLocations) // Specify the navigation property in ServiceAndResource entity
                .HasForeignKey(v => v.SoRId);

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

            modelBuilder.Entity<ReviewAndRating>()
                .HasKey(rr => new { rr.EventId, rr.SoRId,rr.TimeSpan });

            modelBuilder.Entity<EventSoRApprove>()
              .HasKey(e => new { e.EventId, e.SoRId });

            modelBuilder.Entity<ResourceManual>()
                .HasKey(rm => new {rm.SoRId, rm.Manual });

            modelBuilder.Entity<VendorFollow>()
            .HasKey(vf => new { vf.VendorId, vf.ClientId });
        }
    }

 
}
