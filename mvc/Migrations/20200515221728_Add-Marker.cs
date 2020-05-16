using Microsoft.EntityFrameworkCore.Migrations;

namespace mvc.Migrations
{
    public partial class AddMarker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LatitudeMarker",
                table: "Reports",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeMarker",
                table: "Reports",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatitudeMarker",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "LongitudeMarker",
                table: "Reports");
        }
    }
}
