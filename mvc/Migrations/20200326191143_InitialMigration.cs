using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mvc.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfReport = table.Column<DateTime>(nullable: false),
                    HazardLocation = table.Column<string>(nullable: true),
                    HazardDate = table.Column<DateTime>(nullable: false),
                    HazardType = table.Column<string>(nullable: true),
                    HazardDescription = table.Column<string>(nullable: true),
                    ReporterEmail = table.Column<string>(nullable: true),
                    ReporterPhone = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Upvotes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
