﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eventify_backend.Data;

#nullable disable

namespace eventifybackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240318185901_Third migration")]
    partial class Thirdmigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("eventify_backend.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<Guid?>("ClientId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("EndDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("GuestCount")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<byte[]>("Thumbnail")
                        .HasColumnType("longblob");

                    b.HasKey("EventId");

                    b.HasIndex("ClientId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("eventify_backend.Models.EventSR", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("SORId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("Id", "SORId");

                    b.HasIndex("SORId");

                    b.ToTable("EventSr");
                });

            modelBuilder.Entity("eventify_backend.Models.EventSoRApprove", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("SoRId")
                        .HasColumnType("int");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("ReviewAndRatingId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventId", "SoRId");

                    b.HasIndex("ReviewAndRatingId");

                    b.HasIndex("SoRId");

                    b.ToTable("EventSoRApproves");
                });

            modelBuilder.Entity("eventify_backend.Models.FeatureAndFacility", b =>
                {
                    b.Property<int>("SoRId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("FacilityName")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(1);

                    b.HasKey("SoRId", "FacilityName");

                    b.ToTable("FeatureAndFacility");
                });

            modelBuilder.Entity("eventify_backend.Models.Price", b =>
                {
                    b.Property<int>("Pid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("BasePrice")
                        .HasColumnType("double");

                    b.Property<int>("ModelId")
                        .HasColumnType("int");

                    b.Property<string>("Pname")
                        .HasColumnType("longtext");

                    b.HasKey("Pid");

                    b.HasIndex("ModelId")
                        .IsUnique();

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("eventify_backend.Models.PriceModel", b =>
                {
                    b.Property<int>("ModelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ModelName")
                        .HasColumnType("longtext");

                    b.HasKey("ModelId");

                    b.ToTable("PriceModels");
                });

            modelBuilder.Entity("eventify_backend.Models.Rating", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<float>("Ratings")
                        .HasColumnType("float")
                        .HasColumnOrder(1);

                    b.HasKey("Id", "Ratings");

                    b.ToTable("Rating");
                });

            modelBuilder.Entity("eventify_backend.Models.ReviewAndRating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("SoRId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeSpan")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("SoRId");

                    b.ToTable("ReviewAndRatings");
                });

            modelBuilder.Entity("eventify_backend.Models.ReviewContent", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("ReviewAndRatingId")
                        .HasColumnType("int");

                    b.HasKey("Id", "Content");

                    b.HasIndex("ReviewAndRatingId");

                    b.ToTable("ReviewContent");
                });

            modelBuilder.Entity("eventify_backend.Models.ServiceAndResource", b =>
                {
                    b.Property<int>("SoRId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsRequestToDelete")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsSuspend")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<float?>("OverallRate")
                        .HasColumnType("float");

                    b.Property<Guid>("VendorId")
                        .HasColumnType("char(36)");

                    b.HasKey("SoRId");

                    b.HasIndex("VendorId");

                    b.ToTable("ServiceAndResources");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ServiceAndResource");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("eventify_backend.Models.ServiceCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ServiceCategoryName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("CategoryId");

                    b.ToTable("ServiceCategories");
                });

            modelBuilder.Entity("eventify_backend.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("HouseNo")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("ProfilePic")
                        .HasColumnType("longblob");

                    b.Property<string>("Road")
                        .HasColumnType("longtext");

                    b.Property<string>("Street")
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRLocation", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("LocationId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("Area")
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .HasColumnType("longtext");

                    b.Property<string>("District")
                        .HasColumnType("longtext");

                    b.Property<string>("HouseNo")
                        .HasColumnType("longtext");

                    b.Property<string>("State")
                        .HasColumnType("longtext");

                    b.HasKey("Id", "LocationId");

                    b.ToTable("VendorSRLocation");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPhoto", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("photoId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<byte[]>("Image")
                        .HasColumnType("longblob");

                    b.HasKey("Id", "photoId");

                    b.ToTable("VendorSRPhoto");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPrice", b =>
                {
                    b.Property<int>("ServiceAndResourceId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("PriceId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("ServiceAndResourceId", "PriceId");

                    b.HasIndex("PriceId");

                    b.ToTable("VendorSRPrices");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRVideo", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("VideoId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<byte[]>("Video")
                        .HasColumnType("longblob");

                    b.HasKey("Id", "VideoId");

                    b.ToTable("VendorSRVideo");
                });

            modelBuilder.Entity("eventify_backend.Models.Service", b =>
                {
                    b.HasBaseType("eventify_backend.Models.ServiceAndResource");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<int>("ServiceCategoryId")
                        .HasColumnType("int");

                    b.HasIndex("ServiceCategoryId");

                    b.HasDiscriminator().HasValue("Service");
                });

            modelBuilder.Entity("eventify_backend.Models.Client", b =>
                {
                    b.HasBaseType("eventify_backend.Models.User");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.HasDiscriminator().HasValue("Client");
                });

            modelBuilder.Entity("eventify_backend.Models.Vendor", b =>
                {
                    b.HasBaseType("eventify_backend.Models.User");

                    b.Property<string>("CompanyName")
                        .HasColumnType("longtext");

                    b.Property<string>("ContactPersonName")
                        .HasColumnType("longtext");

                    b.HasDiscriminator().HasValue("Vendor");
                });

            modelBuilder.Entity("eventify_backend.Models.Event", b =>
                {
                    b.HasOne("eventify_backend.Models.Client", "Client")
                        .WithMany("Events")
                        .HasForeignKey("ClientId");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("eventify_backend.Models.EventSR", b =>
                {
                    b.HasOne("eventify_backend.Models.Event", "Event")
                        .WithMany("EventSRs")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("EventSRs")
                        .HasForeignKey("SORId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.EventSoRApprove", b =>
                {
                    b.HasOne("eventify_backend.Models.Event", "Event")
                        .WithMany("EventSoRApproves")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.ReviewAndRating", null)
                        .WithMany("EventSoRApprove")
                        .HasForeignKey("ReviewAndRatingId");

                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("EventSoRApproves")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.FeatureAndFacility", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("FeaturesAndFacilities")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.Price", b =>
                {
                    b.HasOne("eventify_backend.Models.PriceModel", "PriceModel")
                        .WithOne("Price")
                        .HasForeignKey("eventify_backend.Models.Price", "ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PriceModel");
                });

            modelBuilder.Entity("eventify_backend.Models.Rating", b =>
                {
                    b.HasOne("eventify_backend.Models.ReviewAndRating", "ReviewAndRating")
                        .WithMany("Ratings")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReviewAndRating");
                });

            modelBuilder.Entity("eventify_backend.Models.ReviewAndRating", b =>
                {
                    b.HasOne("eventify_backend.Models.Event", "Event")
                        .WithMany("ReviewAndRating")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("ReviewAndRating")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.ReviewContent", b =>
                {
                    b.HasOne("eventify_backend.Models.ReviewAndRating", "ReviewAndRating")
                        .WithMany("ReviewAndRatingContents")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.ReviewAndRating", null)
                        .WithMany("ReviewContents")
                        .HasForeignKey("ReviewAndRatingId");

                    b.Navigation("ReviewAndRating");
                });

            modelBuilder.Entity("eventify_backend.Models.ServiceAndResource", b =>
                {
                    b.HasOne("eventify_backend.Models.Vendor", "Vendor")
                        .WithMany("ServiceAndResources")
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRLocation", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorSRLocations")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPhoto", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorRSPhotos")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPrice", b =>
                {
                    b.HasOne("eventify_backend.Models.Price", "Price")
                        .WithMany("VendorSRPrices")
                        .HasForeignKey("PriceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorSRPrices")
                        .HasForeignKey("ServiceAndResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Price");

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRVideo", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorRSVideos")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.Service", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceCategory", "ServiceCategory")
                        .WithMany("Services")
                        .HasForeignKey("ServiceCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceCategory");
                });

            modelBuilder.Entity("eventify_backend.Models.Event", b =>
                {
                    b.Navigation("EventSRs");

                    b.Navigation("EventSoRApproves");

                    b.Navigation("ReviewAndRating");
                });

            modelBuilder.Entity("eventify_backend.Models.Price", b =>
                {
                    b.Navigation("VendorSRPrices");
                });

            modelBuilder.Entity("eventify_backend.Models.PriceModel", b =>
                {
                    b.Navigation("Price");
                });

            modelBuilder.Entity("eventify_backend.Models.ReviewAndRating", b =>
                {
                    b.Navigation("EventSoRApprove");

                    b.Navigation("Ratings");

                    b.Navigation("ReviewAndRatingContents");

                    b.Navigation("ReviewContents");
                });

            modelBuilder.Entity("eventify_backend.Models.ServiceAndResource", b =>
                {
                    b.Navigation("EventSRs");

                    b.Navigation("EventSoRApproves");

                    b.Navigation("FeaturesAndFacilities");

                    b.Navigation("ReviewAndRating");

                    b.Navigation("VendorRSPhotos");

                    b.Navigation("VendorRSVideos");

                    b.Navigation("VendorSRLocations");

                    b.Navigation("VendorSRPrices");
                });

            modelBuilder.Entity("eventify_backend.Models.ServiceCategory", b =>
                {
                    b.Navigation("Services");
                });

            modelBuilder.Entity("eventify_backend.Models.Client", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("eventify_backend.Models.Vendor", b =>
                {
                    b.Navigation("ServiceAndResources");
                });
#pragma warning restore 612, 618
        }
    }
}
