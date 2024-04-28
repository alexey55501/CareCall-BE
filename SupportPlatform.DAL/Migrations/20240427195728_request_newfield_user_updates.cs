using Microsoft.EntityFrameworkCore.Migrations;

namespace SupportPlatform.API.DAL.Migrations
{
    public partial class request_newfield_user_updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventDate",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventTime",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EventTime",
                table: "Requests");
        }
    }
}
