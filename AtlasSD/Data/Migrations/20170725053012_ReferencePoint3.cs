using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtlasSD.Data.Migrations
{
    public partial class ReferencePoint3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferencePoint_Bloc_BlocId1",
                table: "ReferencePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferencePoint_Group_GroupId1",
                table: "ReferencePoint");

            migrationBuilder.DropIndex(
                name: "IX_ReferencePoint_BlocId1",
                table: "ReferencePoint");

            migrationBuilder.DropIndex(
                name: "IX_ReferencePoint_GroupId1",
                table: "ReferencePoint");

            migrationBuilder.DropColumn(
                name: "BlocId1",
                table: "ReferencePoint");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "ReferencePoint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlocId1",
                table: "ReferencePoint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId1",
                table: "ReferencePoint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePoint_BlocId1",
                table: "ReferencePoint",
                column: "BlocId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePoint_GroupId1",
                table: "ReferencePoint",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferencePoint_Bloc_BlocId1",
                table: "ReferencePoint",
                column: "BlocId1",
                principalTable: "Bloc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferencePoint_Group_GroupId1",
                table: "ReferencePoint",
                column: "GroupId1",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
