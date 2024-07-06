using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDS.Migrations
{
    /// <inheritdoc />
    public partial class orderinitagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_DrugstoreId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DrugstoreId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DrugstoreId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DrugstoreId",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DrugstoreId",
                table: "Orders",
                column: "DrugstoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_DrugstoreId",
                table: "Orders",
                column: "DrugstoreId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
