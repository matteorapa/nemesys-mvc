using Microsoft.EntityFrameworkCore.Migrations;

namespace mvc.Migrations
{
    public partial class UpdateUserUpvoteRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Upvotes_UserId",
                table: "Upvotes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_UserId",
                table: "Upvotes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Upvotes_UserId",
                table: "Upvotes");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Upvotes_UserId",
                table: "Upvotes",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }
    }
}
