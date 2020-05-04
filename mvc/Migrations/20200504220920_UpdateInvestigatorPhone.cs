using Microsoft.EntityFrameworkCore.Migrations;

namespace mvc.Migrations
{
    public partial class UpdateInvestigatorPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportStatus",
                table: "Reports",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvestigatorPhone",
                table: "Investigations",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportStatus",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "InvestigatorPhone",
                table: "Investigations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
