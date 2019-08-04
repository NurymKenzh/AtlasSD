using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Map7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Colors",
                table: "Map",
                nullable: true);

            migrationBuilder.AddColumn<decimal[]>(
                name: "MinValues",
                table: "Map",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colors",
                table: "Map");

            migrationBuilder.DropColumn(
                name: "MinValues",
                table: "Map");
        }
    }
}
