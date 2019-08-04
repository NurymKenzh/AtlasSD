using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Map9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionEN",
                table: "Map",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionKK",
                table: "Map",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionRU",
                table: "Map",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEN",
                table: "Map");

            migrationBuilder.DropColumn(
                name: "DescriptionKK",
                table: "Map");

            migrationBuilder.DropColumn(
                name: "DescriptionRU",
                table: "Map");
        }
    }
}
