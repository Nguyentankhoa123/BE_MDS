using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ProductId",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductId",
                table: "Comments",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ProductId",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductId",
                table: "Comments",
                column: "ProductId",
                unique: true);
        }
    }
}
