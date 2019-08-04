using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AtlasSD.Data.Migrations
{
    public partial class Region1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionData");

            migrationBuilder.AddColumn<decimal>(
                name: "Area",
                table: "Region",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Coordinates",
                table: "Region",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Population",
                table: "Region",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Region",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "Coordinates",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "Population",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Region");

            migrationBuilder.CreateTable(
                name: "RegionData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Area = table.Column<decimal>(nullable: false),
                    Coordinates = table.Column<decimal>(nullable: false),
                    Population = table.Column<decimal>(nullable: false),
                    RegionId = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionData_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionData_RegionId",
                table: "RegionData",
                column: "RegionId");
        }
    }
}
