using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventifybackend.Migrations
{
    /// <inheritdoc />
    public partial class Thirdmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSoRApprove_Events_EventId",
                table: "EventSoRApprove");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSoRApprove_ReviewAndRatings_ReviewAndRatingId",
                table: "EventSoRApprove");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSoRApprove_ServiceAndResources_SoRId",
                table: "EventSoRApprove");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventSoRApprove",
                table: "EventSoRApprove");

            migrationBuilder.RenameTable(
                name: "EventSoRApprove",
                newName: "EventSoRApproves");

            migrationBuilder.RenameIndex(
                name: "IX_EventSoRApprove_SoRId",
                table: "EventSoRApproves",
                newName: "IX_EventSoRApproves_SoRId");

            migrationBuilder.RenameIndex(
                name: "IX_EventSoRApprove_ReviewAndRatingId",
                table: "EventSoRApproves",
                newName: "IX_EventSoRApproves_ReviewAndRatingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventSoRApproves",
                table: "EventSoRApproves",
                columns: new[] { "EventId", "SoRId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventSoRApproves_Events_EventId",
                table: "EventSoRApproves",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSoRApproves_ReviewAndRatings_ReviewAndRatingId",
                table: "EventSoRApproves",
                column: "ReviewAndRatingId",
                principalTable: "ReviewAndRatings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSoRApproves_ServiceAndResources_SoRId",
                table: "EventSoRApproves",
                column: "SoRId",
                principalTable: "ServiceAndResources",
                principalColumn: "SoRId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventSoRApproves_Events_EventId",
                table: "EventSoRApproves");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSoRApproves_ReviewAndRatings_ReviewAndRatingId",
                table: "EventSoRApproves");

            migrationBuilder.DropForeignKey(
                name: "FK_EventSoRApproves_ServiceAndResources_SoRId",
                table: "EventSoRApproves");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventSoRApproves",
                table: "EventSoRApproves");

            migrationBuilder.RenameTable(
                name: "EventSoRApproves",
                newName: "EventSoRApprove");

            migrationBuilder.RenameIndex(
                name: "IX_EventSoRApproves_SoRId",
                table: "EventSoRApprove",
                newName: "IX_EventSoRApprove_SoRId");

            migrationBuilder.RenameIndex(
                name: "IX_EventSoRApproves_ReviewAndRatingId",
                table: "EventSoRApprove",
                newName: "IX_EventSoRApprove_ReviewAndRatingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventSoRApprove",
                table: "EventSoRApprove",
                columns: new[] { "EventId", "SoRId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventSoRApprove_Events_EventId",
                table: "EventSoRApprove",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventSoRApprove_ReviewAndRatings_ReviewAndRatingId",
                table: "EventSoRApprove",
                column: "ReviewAndRatingId",
                principalTable: "ReviewAndRatings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventSoRApprove_ServiceAndResources_SoRId",
                table: "EventSoRApprove",
                column: "SoRId",
                principalTable: "ServiceAndResources",
                principalColumn: "SoRId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
