using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDS.Migrations
{
    /// <inheritdoc />
    public partial class AddisReviewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isReviewed",
                table: "OrderDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isReviewed",
                table: "OrderDetails");
        }
    }
}
