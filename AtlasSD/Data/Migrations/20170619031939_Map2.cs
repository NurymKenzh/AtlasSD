using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Map2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndicatorIds",
                table: "Map");

            migrationBuilder.AddColumn<int>(
                name: "IndicatorId",
                table: "Map",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndicatorId",
                table: "Map");

            migrationBuilder.AddColumn<List<int>>(
                name: "IndicatorIds",
                table: "Map",
                nullable: true);
        }
    }
}
