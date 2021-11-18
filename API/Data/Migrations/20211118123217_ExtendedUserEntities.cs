using Microsoft.EntityFrameworkCore.Migrations;

namespace API.data.Migrations
{
    public partial class ExtendedUserEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LookingFro",
                table: "AppUsers",
                newName: "LookingFor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LookingFor",
                table: "AppUsers",
                newName: "LookingFro");
        }
    }
}
