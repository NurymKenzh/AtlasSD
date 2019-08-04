using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class IndicatorValue2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "IndicatorValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorValue_RegionId",
                table: "IndicatorValue",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_IndicatorValue_Region_RegionId",
                table: "IndicatorValue",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndicatorValue_Region_RegionId",
                table: "IndicatorValue");

            migrationBuilder.DropIndex(
                name: "IX_IndicatorValue_RegionId",
                table: "IndicatorValue");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "IndicatorValue");
        }
    }
}
