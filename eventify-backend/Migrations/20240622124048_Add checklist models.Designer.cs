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
    [Migration("20240622124048_Add checklist models")]
    partial class Addchecklistmodels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PriceVendorSRPrice", b =>
                {
                    b.Property<int>("PricePid")
                        .HasColumnType("int");

                    b.Property<int>("VendorSRPricesSoRId")
                        .HasColumnType("int");

                    b.Property<int>("VendorSRPricesPId")
                        .HasColumnType("int");

                    b.HasKey("PricePid", "VendorSRPricesSoRId", "VendorSRPricesPId");

                    b.HasIndex("VendorSRPricesSoRId", "VendorSRPricesPId");

                    b.ToTable("PriceVendorSRPrice");
                });

            modelBuilder.Entity("eventify_backend.Models.Agenda", b =>
                {
                    b.Property<int>("AgendaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("AgendaId");

                    b.HasIndex("EventId")
                        .IsUnique();

                    b.ToTable("Agenda");
                });

            modelBuilder.Entity("eventify_backend.Models.AgendaTask", b =>
                {
                    b.Property<int>("AgendaTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AgendaId")
                        .HasColumnType("int");

                    b.Property<string>("TaskDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("time(6)");

                    b.HasKey("AgendaTaskId");

                    b.HasIndex("AgendaId");

                    b.ToTable("AgendaTask");
                });

            modelBuilder.Entity("eventify_backend.Models.Checklist", b =>
                {
                    b.Property<int>("ChecklistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ChecklistId");

                    b.HasIndex("EventId")
                        .IsUnique();

                    b.ToTable("Checklist");
                });

            modelBuilder.Entity("eventify_backend.Models.ChecklistTask", b =>
                {
                    b.Property<int>("ChecklistTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Checked")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("ChecklistId")
                        .HasColumnType("int");

                    b.Property<string>("TaskDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ChecklistTaskId");

                    b.HasIndex("ChecklistId");

                    b.ToTable("ChecklistTask");
                });

            modelBuilder.Entity("eventify_backend.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<Guid>("ClientId")
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

                    b.Property<string>("Thumbnail")
                        .HasColumnType("longtext");

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

                    b.Property<bool>("IsApprove")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventId", "SoRId");

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

            modelBuilder.Entity("eventify_backend.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Read")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("Notification");
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

                    b.HasIndex("ModelId");

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

            modelBuilder.Entity("eventify_backend.Models.ResourceCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ResourceCategoryName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("CategoryId");

                    b.ToTable("ResourceCategories");
                });

            modelBuilder.Entity("eventify_backend.Models.ResourceManual", b =>
                {
                    b.Property<int>("SoRId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("Manual")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(1);

                    b.HasKey("SoRId", "Manual");

                    b.ToTable("ResourceManual");
                });

            modelBuilder.Entity("eventify_backend.Models.ReviewAndRating", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("SoRId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<float>("Ratings")
                        .HasColumnType("float");

                    b.Property<DateTime>("TimeSpan")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventId", "SoRId");

                    b.HasIndex("SoRId");

                    b.ToTable("ReviewAndRatings");
                });

            modelBuilder.Entity("eventify_backend.Models.ServiceAndResource", b =>
                {
                    b.Property<int>("SoRId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Capacity")
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
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("HouseNo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ProfilePic")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ResetPasswordToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ResetPasswordTokenExpiryTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Road")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("User");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("eventify_backend.Models.VendorFollow", b =>
                {
                    b.Property<Guid>("VendorId")
                        .HasColumnType("char(36)")
                        .HasColumnOrder(0);

                    b.Property<Guid>("ClientId")
                        .HasColumnType("char(36)")
                        .HasColumnOrder(1);

                    b.HasKey("VendorId", "ClientId");

                    b.HasIndex("ClientId");

                    b.ToTable("VendorFollows");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRLocation", b =>
                {
                    b.Property<int>("SoRId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("HouseNo")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(1);

                    b.Property<string>("Area")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(2);

                    b.Property<string>("District")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(3);

                    b.Property<string>("Country")
                        .HasColumnType("longtext");

                    b.Property<string>("State")
                        .HasColumnType("longtext");

                    b.HasKey("SoRId", "HouseNo", "Area", "District");

                    b.ToTable("VendorSRLocation");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPhoto", b =>
                {
                    b.Property<int>("SoRId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("Image")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(1);

                    b.HasKey("SoRId", "Image");

                    b.ToTable("VendorSRPhoto");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPrice", b =>
                {
                    b.Property<int>("SoRId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("PId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("SoRId", "PId");

                    b.ToTable("VendorSRPrices");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRVideo", b =>
                {
                    b.Property<int>("SoRId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<string>("Video")
                        .HasColumnType("varchar(255)")
                        .HasColumnOrder(1);

                    b.HasKey("SoRId", "Video");

                    b.ToTable("VendorSRVideo");
                });

            modelBuilder.Entity("eventify_backend.Models.Resource", b =>
                {
                    b.HasBaseType("eventify_backend.Models.ServiceAndResource");

                    b.Property<int>("ResourceCategoryId")
                        .HasColumnType("int");

                    b.HasIndex("ResourceCategoryId");

                    b.HasDiscriminator().HasValue("Resource");
                });

            modelBuilder.Entity("eventify_backend.Models.Service", b =>
                {
                    b.HasBaseType("eventify_backend.Models.ServiceAndResource");

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

            modelBuilder.Entity("PriceVendorSRPrice", b =>
                {
                    b.HasOne("eventify_backend.Models.Price", null)
                        .WithMany()
                        .HasForeignKey("PricePid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.VendorSRPrice", null)
                        .WithMany()
                        .HasForeignKey("VendorSRPricesSoRId", "VendorSRPricesPId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("eventify_backend.Models.Agenda", b =>
                {
                    b.HasOne("eventify_backend.Models.Event", "Event")
                        .WithOne("Agenda")
                        .HasForeignKey("eventify_backend.Models.Agenda", "EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("eventify_backend.Models.AgendaTask", b =>
                {
                    b.HasOne("eventify_backend.Models.Agenda", "Agenda")
                        .WithMany("AgendaTasks")
                        .HasForeignKey("AgendaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agenda");
                });

            modelBuilder.Entity("eventify_backend.Models.Checklist", b =>
                {
                    b.HasOne("eventify_backend.Models.Event", "Event")
                        .WithOne("Checklist")
                        .HasForeignKey("eventify_backend.Models.Checklist", "EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("eventify_backend.Models.ChecklistTask", b =>
                {
                    b.HasOne("eventify_backend.Models.Checklist", "Checklist")
                        .WithMany("ChecklistTasks")
                        .HasForeignKey("ChecklistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Checklist");
                });

            modelBuilder.Entity("eventify_backend.Models.Event", b =>
                {
                    b.HasOne("eventify_backend.Models.Client", "Client")
                        .WithMany("Events")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

            modelBuilder.Entity("eventify_backend.Models.Notification", b =>
                {
                    b.HasOne("eventify_backend.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("eventify_backend.Models.Price", b =>
                {
                    b.HasOne("eventify_backend.Models.PriceModel", "PriceModel")
                        .WithMany("Price")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PriceModel");
                });

            modelBuilder.Entity("eventify_backend.Models.ResourceManual", b =>
                {
                    b.HasOne("eventify_backend.Models.Resource", "Resource")
                        .WithMany("ResourceManual")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
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

            modelBuilder.Entity("eventify_backend.Models.ServiceAndResource", b =>
                {
                    b.HasOne("eventify_backend.Models.Vendor", "Vendor")
                        .WithMany("ServiceAndResources")
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorFollow", b =>
                {
                    b.HasOne("eventify_backend.Models.Client", "Client")
                        .WithMany("VendorFollows")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("eventify_backend.Models.Vendor", "Vendor")
                        .WithMany("VendorFollows")
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRLocation", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorSRLocations")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPhoto", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorRSPhotos")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRPrice", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorSRPrices")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.VendorSRVideo", b =>
                {
                    b.HasOne("eventify_backend.Models.ServiceAndResource", "ServiceAndResource")
                        .WithMany("VendorRSVideos")
                        .HasForeignKey("SoRId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceAndResource");
                });

            modelBuilder.Entity("eventify_backend.Models.Resource", b =>
                {
                    b.HasOne("eventify_backend.Models.ResourceCategory", "ResourceCategory")
                        .WithMany("Resources")
                        .HasForeignKey("ResourceCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResourceCategory");
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

            modelBuilder.Entity("eventify_backend.Models.Agenda", b =>
                {
                    b.Navigation("AgendaTasks");
                });

            modelBuilder.Entity("eventify_backend.Models.Checklist", b =>
                {
                    b.Navigation("ChecklistTasks");
                });

            modelBuilder.Entity("eventify_backend.Models.Event", b =>
                {
                    b.Navigation("Agenda");

                    b.Navigation("Checklist");

                    b.Navigation("EventSRs");

                    b.Navigation("EventSoRApproves");

                    b.Navigation("ReviewAndRating");
                });

            modelBuilder.Entity("eventify_backend.Models.PriceModel", b =>
                {
                    b.Navigation("Price");
                });

            modelBuilder.Entity("eventify_backend.Models.ResourceCategory", b =>
                {
                    b.Navigation("Resources");
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

            modelBuilder.Entity("eventify_backend.Models.User", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("eventify_backend.Models.Resource", b =>
                {
                    b.Navigation("ResourceManual");
                });

            modelBuilder.Entity("eventify_backend.Models.Client", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("VendorFollows");
                });

            modelBuilder.Entity("eventify_backend.Models.Vendor", b =>
                {
                    b.Navigation("ServiceAndResources");

                    b.Navigation("VendorFollows");
                });
#pragma warning restore 612, 618
        }
    }
}
