using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDS.Migrations
{
    /// <inheritdoc />
    public partial class OrderDetailCreateOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateOn",
                table: "OrderDetails",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateOn",
                table: "OrderDetails");
        }
    }
}
