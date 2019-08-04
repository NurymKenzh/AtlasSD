using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Map5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MapId",
                table: "Indicator",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Indicator_MapId",
                table: "Indicator",
                column: "MapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Indicator_Map_MapId",
                table: "Indicator",
                column: "MapId",
                principalTable: "Map",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicator_Map_MapId",
                table: "Indicator");

            migrationBuilder.DropIndex(
                name: "IX_Indicator_MapId",
                table: "Indicator");

            migrationBuilder.DropColumn(
                name: "MapId",
                table: "Indicator");
        }
    }
}
