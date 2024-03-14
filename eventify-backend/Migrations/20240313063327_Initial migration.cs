using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventifybackend.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceModels",
                columns: table => new
                {
                    ModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModelName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceModels", x => x.ModelId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServiceCategoryName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.CategoryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilePic = table.Column<byte[]>(type: "longblob", nullable: true),
                    HouseNo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Road = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactPersonName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Pid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pname = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BasePrice = table.Column<double>(type: "double", nullable: false),
                    ModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Pid);
                    table.ForeignKey(
                        name: "FK_Prices_PriceModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "PriceModels",
                        principalColumn: "ModelId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GuestCount = table.Column<int>(type: "int", nullable: false),
                    Thumbnail = table.Column<byte[]>(type: "longblob", nullable: true),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServiceAndResources",
                columns: table => new
                {
                    SoRId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSuspend = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsRequestToDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OverallRate = table.Column<float>(type: "float", nullable: true),
                    VendorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    ServiceCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAndResources", x => x.SoRId);
                    table.ForeignKey(
                        name: "FK_ServiceAndResources_ServiceCategories_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceAndResources_Users_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventSr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SORId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSr", x => new { x.Id, x.SORId });
                    table.ForeignKey(
                        name: "FK_EventSr_Events_Id",
                        column: x => x.Id,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSr_ServiceAndResources_SORId",
                        column: x => x.SORId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeatureAndFacility",
                columns: table => new
                {
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    FacilityName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureAndFacility", x => new { x.SoRId, x.FacilityName });
                    table.ForeignKey(
                        name: "FK_FeatureAndFacility_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReviewAndRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    TimeSpan = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewAndRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewAndRatings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewAndRatings_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRLocation",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    HouseNo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Area = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    District = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRLocation", x => new { x.Id, x.LocationId });
                    table.ForeignKey(
                        name: "FK_VendorSRLocation_ServiceAndResources_Id",
                        column: x => x.Id,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    photoId = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRPhoto", x => new { x.Id, x.photoId });
                    table.ForeignKey(
                        name: "FK_VendorSRPhoto_ServiceAndResources_Id",
                        column: x => x.Id,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRPrices",
                columns: table => new
                {
                    ServiceAndResourceId = table.Column<int>(type: "int", nullable: false),
                    PriceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRPrices", x => new { x.ServiceAndResourceId, x.PriceId });
                    table.ForeignKey(
                        name: "FK_VendorSRPrices_Prices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Prices",
                        principalColumn: "Pid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorSRPrices_ServiceAndResources_ServiceAndResourceId",
                        column: x => x.ServiceAndResourceId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRVideo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<int>(type: "int", nullable: false),
                    Video = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRVideo", x => new { x.Id, x.VideoId });
                    table.ForeignKey(
                        name: "FK_VendorSRVideo_ServiceAndResources_Id",
                        column: x => x.Id,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventSoRApprove",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReviewAndRatingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSoRApprove", x => new { x.EventId, x.SoRId });
                    table.ForeignKey(
                        name: "FK_EventSoRApprove_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSoRApprove_ReviewAndRatings_ReviewAndRatingId",
                        column: x => x.ReviewAndRatingId,
                        principalTable: "ReviewAndRatings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventSoRApprove_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Ratings = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => new { x.Id, x.Ratings });
                    table.ForeignKey(
                        name: "FK_Rating_ReviewAndRatings_Id",
                        column: x => x.Id,
                        principalTable: "ReviewAndRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReviewContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReviewAndRatingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewContent", x => new { x.Id, x.Content });
                    table.ForeignKey(
                        name: "FK_ReviewContent_ReviewAndRatings_Id",
                        column: x => x.Id,
                        principalTable: "ReviewAndRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewContent_ReviewAndRatings_ReviewAndRatingId",
                        column: x => x.ReviewAndRatingId,
                        principalTable: "ReviewAndRatings",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ClientId",
                table: "Events",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSoRApprove_ReviewAndRatingId",
                table: "EventSoRApprove",
                column: "ReviewAndRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSoRApprove_SoRId",
                table: "EventSoRApprove",
                column: "SoRId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSr_SORId",
                table: "EventSr",
                column: "SORId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ModelId",
                table: "Prices",
                column: "ModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAndRatings_EventId",
                table: "ReviewAndRatings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAndRatings_SoRId",
                table: "ReviewAndRatings",
                column: "SoRId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewContent_ReviewAndRatingId",
                table: "ReviewContent",
                column: "ReviewAndRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAndResources_ServiceCategoryId",
                table: "ServiceAndResources",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAndResources_VendorId",
                table: "ServiceAndResources",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorSRPrices_PriceId",
                table: "VendorSRPrices",
                column: "PriceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventSoRApprove");

            migrationBuilder.DropTable(
                name: "EventSr");

            migrationBuilder.DropTable(
                name: "FeatureAndFacility");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "ReviewContent");

            migrationBuilder.DropTable(
                name: "VendorSRLocation");

            migrationBuilder.DropTable(
                name: "VendorSRPhoto");

            migrationBuilder.DropTable(
                name: "VendorSRPrices");

            migrationBuilder.DropTable(
                name: "VendorSRVideo");

            migrationBuilder.DropTable(
                name: "ReviewAndRatings");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ServiceAndResources");

            migrationBuilder.DropTable(
                name: "PriceModels");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
