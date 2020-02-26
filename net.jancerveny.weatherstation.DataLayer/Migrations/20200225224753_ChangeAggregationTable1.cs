using Microsoft.EntityFrameworkCore.Migrations;

namespace net.jancerveny.weatherstation.DataLayer.Migrations
{
    public partial class ChangeAggregationTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AggregationLength",
                schema: "Weather",
                table: "aggregated_measurements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AggregationLength",
                schema: "Weather",
                table: "aggregated_measurements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
