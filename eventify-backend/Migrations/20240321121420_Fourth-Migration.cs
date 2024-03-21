using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eventifybackend.Migrations
{
    /// <inheritdoc />
    public partial class FourthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "EventSoRApproves",
                newName: "IsRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRequest",
                table: "EventSoRApproves",
                newName: "IsApproved");
        }
    }
}
