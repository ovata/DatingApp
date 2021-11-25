using Microsoft.EntityFrameworkCore.Migrations;

namespace API.data.Migrations
{
    public partial class interest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interest",
                table: "AppUsers",
                newName: "Interests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interests",
                table: "AppUsers",
                newName: "Interest");
        }
    }
}
