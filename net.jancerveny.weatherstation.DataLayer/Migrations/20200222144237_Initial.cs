using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace net.jancerveny.weatherstation.DataLayer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Weather");

            migrationBuilder.CreateTable(
                name: "data_sources",
                schema: "Weather",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    SourceType = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRead = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aggregated_measurements",
                schema: "Weather",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    Temperature = table.Column<int>(nullable: false),
                    AggregationLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aggregated_measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aggregated_measurements_data_sources_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "Weather",
                        principalTable: "data_sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "measurements",
                schema: "Weather",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    Temperature = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_measurements_data_sources_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "Weather",
                        principalTable: "data_sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aggregated_measurements_SourceId",
                schema: "Weather",
                table: "aggregated_measurements",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_measurements_SourceId",
                schema: "Weather",
                table: "measurements",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aggregated_measurements",
                schema: "Weather");

            migrationBuilder.DropTable(
                name: "measurements",
                schema: "Weather");

            migrationBuilder.DropTable(
                name: "data_sources",
                schema: "Weather");
        }
    }
}
