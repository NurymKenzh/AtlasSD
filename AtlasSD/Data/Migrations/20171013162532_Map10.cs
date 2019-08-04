using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Map10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeywordsEN",
                table: "Map",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeywordsKK",
                table: "Map",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeywordsRU",
                table: "Map",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeywordsEN",
                table: "Map");

            migrationBuilder.DropColumn(
                name: "KeywordsKK",
                table: "Map");

            migrationBuilder.DropColumn(
                name: "KeywordsRU",
                table: "Map");
        }
    }
}
