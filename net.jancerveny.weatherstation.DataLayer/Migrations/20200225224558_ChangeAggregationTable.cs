using Microsoft.EntityFrameworkCore.Migrations;

namespace net.jancerveny.weatherstation.DataLayer.Migrations
{
    public partial class ChangeAggregationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                schema: "Weather",
                table: "aggregated_measurements",
                newName: "Day");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Day",
                schema: "Weather",
                table: "aggregated_measurements",
                newName: "Timestamp");
        }
    }
}
