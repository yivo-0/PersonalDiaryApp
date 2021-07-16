using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLibrary.Migrations
{
    public partial class AddImageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Records");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageByte",
                table: "Records",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageByte",
                table: "Records");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Records",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
