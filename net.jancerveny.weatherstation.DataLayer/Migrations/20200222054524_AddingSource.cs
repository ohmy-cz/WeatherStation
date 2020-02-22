using Microsoft.EntityFrameworkCore.Migrations;

namespace net.jancerveny.weatherstation.DataLayer.Migrations
{
    public partial class AddingSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                schema: "Weather",
                table: "temperatures",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                schema: "Weather",
                table: "temperatures");
        }
    }
}
