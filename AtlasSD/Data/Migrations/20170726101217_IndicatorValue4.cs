using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class IndicatorValue4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "IndicatorValue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorValue_SourceId",
                table: "IndicatorValue",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_IndicatorValue_Source_SourceId",
                table: "IndicatorValue",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndicatorValue_Source_SourceId",
                table: "IndicatorValue");

            migrationBuilder.DropIndex(
                name: "IX_IndicatorValue_SourceId",
                table: "IndicatorValue");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "IndicatorValue");
        }
    }
}
