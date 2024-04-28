using Microsoft.EntityFrameworkCore.Migrations;

namespace SupportPlatform.API.DAL.Migrations
{
    public partial class locationjson_rn_address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationJSON",
                table: "Requests",
                newName: "Address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Requests",
                newName: "LocationJSON");
        }
    }
}
