using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Region4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Population",
                table: "Region",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateIndex(
                name: "IX_Map_IndicatorId",
                table: "Map",
                column: "IndicatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Map_Indicator_IndicatorId",
                table: "Map",
                column: "IndicatorId",
                principalTable: "Indicator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Map_Indicator_IndicatorId",
                table: "Map");

            migrationBuilder.DropIndex(
                name: "IX_Map_IndicatorId",
                table: "Map");

            migrationBuilder.AlterColumn<decimal>(
                name: "Population",
                table: "Region",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
