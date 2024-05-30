using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventifybackend.Migrations
{
    /// <inheritdoc />
    public partial class Addrole : Migration
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
                name: "ResourceCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ResourceCategoryName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCategories", x => x.CategoryId);
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
                    Role = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilePic = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
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
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResourceCategoryId = table.Column<int>(type: "int", nullable: true),
                    ServiceCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAndResources", x => x.SoRId);
                    table.ForeignKey(
                        name: "FK_ServiceAndResources_ResourceCategories_ResourceCategoryId",
                        column: x => x.ResourceCategoryId,
                        principalTable: "ResourceCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "VendorFollows",
                columns: table => new
                {
                    VendorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorFollows", x => new { x.VendorId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_VendorFollows_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorFollows_Users_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventSoRApproves",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsApprove = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSoRApproves", x => new { x.EventId, x.SoRId });
                    table.ForeignKey(
                        name: "FK_EventSoRApproves_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventSoRApproves_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
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
                name: "ResourceManual",
                columns: table => new
                {
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    Manual = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceManual", x => new { x.SoRId, x.Manual });
                    table.ForeignKey(
                        name: "FK_ResourceManual_ServiceAndResources_SoRId",
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
                    EventId = table.Column<int>(type: "int", nullable: false),
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    Ratings = table.Column<float>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TimeSpan = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewAndRatings", x => new { x.EventId, x.SoRId });
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
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    HouseNo = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Area = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    District = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRLocation", x => new { x.SoRId, x.HouseNo, x.Area, x.District });
                    table.ForeignKey(
                        name: "FK_VendorSRLocation_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRPhoto",
                columns: table => new
                {
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRPhoto", x => new { x.SoRId, x.Image });
                    table.ForeignKey(
                        name: "FK_VendorSRPhoto_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRPrices",
                columns: table => new
                {
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    PId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRPrices", x => new { x.SoRId, x.PId });
                    table.ForeignKey(
                        name: "FK_VendorSRPrices_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VendorSRVideo",
                columns: table => new
                {
                    SoRId = table.Column<int>(type: "int", nullable: false),
                    Video = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorSRVideo", x => new { x.SoRId, x.Video });
                    table.ForeignKey(
                        name: "FK_VendorSRVideo_ServiceAndResources_SoRId",
                        column: x => x.SoRId,
                        principalTable: "ServiceAndResources",
                        principalColumn: "SoRId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PriceVendorSRPrice",
                columns: table => new
                {
                    PricePid = table.Column<int>(type: "int", nullable: false),
                    VendorSRPricesSoRId = table.Column<int>(type: "int", nullable: false),
                    VendorSRPricesPId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceVendorSRPrice", x => new { x.PricePid, x.VendorSRPricesSoRId, x.VendorSRPricesPId });
                    table.ForeignKey(
                        name: "FK_PriceVendorSRPrice_Prices_PricePid",
                        column: x => x.PricePid,
                        principalTable: "Prices",
                        principalColumn: "Pid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceVendorSRPrice_VendorSRPrices_VendorSRPricesSoRId_Vendor~",
                        columns: x => new { x.VendorSRPricesSoRId, x.VendorSRPricesPId },
                        principalTable: "VendorSRPrices",
                        principalColumns: new[] { "SoRId", "PId" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ClientId",
                table: "Events",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSoRApproves_SoRId",
                table: "EventSoRApproves",
                column: "SoRId");

            migrationBuilder.CreateIndex(
                name: "IX_EventSr_SORId",
                table: "EventSr",
                column: "SORId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_ModelId",
                table: "Prices",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceVendorSRPrice_VendorSRPricesSoRId_VendorSRPricesPId",
                table: "PriceVendorSRPrice",
                columns: new[] { "VendorSRPricesSoRId", "VendorSRPricesPId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAndRatings_SoRId",
                table: "ReviewAndRatings",
                column: "SoRId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAndResources_ResourceCategoryId",
                table: "ServiceAndResources",
                column: "ResourceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAndResources_ServiceCategoryId",
                table: "ServiceAndResources",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAndResources_VendorId",
                table: "ServiceAndResources",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorFollows_ClientId",
                table: "VendorFollows",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventSoRApproves");

            migrationBuilder.DropTable(
                name: "EventSr");

            migrationBuilder.DropTable(
                name: "FeatureAndFacility");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PriceVendorSRPrice");

            migrationBuilder.DropTable(
                name: "ResourceManual");

            migrationBuilder.DropTable(
                name: "ReviewAndRatings");

            migrationBuilder.DropTable(
                name: "VendorFollows");

            migrationBuilder.DropTable(
                name: "VendorSRLocation");

            migrationBuilder.DropTable(
                name: "VendorSRPhoto");

            migrationBuilder.DropTable(
                name: "VendorSRVideo");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "VendorSRPrices");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "PriceModels");

            migrationBuilder.DropTable(
                name: "ServiceAndResources");

            migrationBuilder.DropTable(
                name: "ResourceCategories");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
