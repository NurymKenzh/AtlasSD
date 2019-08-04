using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class IndicatorValue5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndicatorValue_Source_SourceId",
                table: "IndicatorValue");

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "IndicatorValue",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_IndicatorValue_Source_SourceId",
                table: "IndicatorValue",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndicatorValue_Source_SourceId",
                table: "IndicatorValue");

            migrationBuilder.AlterColumn<int>(
                name: "SourceId",
                table: "IndicatorValue",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IndicatorValue_Source_SourceId",
                table: "IndicatorValue",
                column: "SourceId",
                principalTable: "Source",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
