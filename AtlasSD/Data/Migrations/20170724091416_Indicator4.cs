using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Indicator4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "Indicator");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "Indicator");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Indicator",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Indicator");

            migrationBuilder.AddColumn<decimal>(
                name: "Max",
                table: "Indicator",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Min",
                table: "Indicator",
                nullable: true);
        }
    }
}
