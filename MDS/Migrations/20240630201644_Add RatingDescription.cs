using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDS.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RatingDescription",
                table: "FeedBacks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingDescription",
                table: "FeedBacks");
        }
    }
}
