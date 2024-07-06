using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDS.Migrations
{
    /// <inheritdoc />
    public partial class OrderDetailinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DrugstoreId",
                table: "OrderDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DrugstoreId",
                table: "OrderDetails",
                column: "DrugstoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_AspNetUsers_DrugstoreId",
                table: "OrderDetails",
                column: "DrugstoreId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_AspNetUsers_DrugstoreId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_DrugstoreId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DrugstoreId",
                table: "OrderDetails");
        }
    }
}
