using Microsoft.EntityFrameworkCore.Migrations;

namespace mvc.Migrations
{
    public partial class AddInvestigationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Investigations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Investigations_UserId",
                table: "Investigations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investigations_AspNetUsers_UserId",
                table: "Investigations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investigations_AspNetUsers_UserId",
                table: "Investigations");

            migrationBuilder.DropIndex(
                name: "IX_Investigations_UserId",
                table: "Investigations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Investigations");
        }
    }
}
