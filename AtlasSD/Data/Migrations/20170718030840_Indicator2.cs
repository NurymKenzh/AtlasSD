using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class Indicator2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "Indicator",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Max",
                table: "Indicator",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Min",
                table: "Indicator",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Formula",
                table: "Indicator");

            migrationBuilder.DropColumn(
                name: "Max",
                table: "Indicator");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "Indicator");
        }
    }
}
