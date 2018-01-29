using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace NackademinUppgift07.Migrations
{
    public partial class PizzaPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Totalbelopp",
                table: "Bestallning",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "GratisPizzaPris",
                table: "Bestallning",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrdinalBelopp",
                table: "Bestallning",
                type: "int",
                nullable: false,
                defaultValue: 0);

	        migrationBuilder.Sql(@"UPDATE Bestallning SET OrdinalBelopp=Totalbelopp");

            migrationBuilder.AddColumn<decimal>(
                name: "Rabatt",
                table: "Bestallning",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GratisPizzaPris",
                table: "Bestallning");

            migrationBuilder.DropColumn(
                name: "OrdinalBelopp",
                table: "Bestallning");

            migrationBuilder.DropColumn(
                name: "Rabatt",
                table: "Bestallning");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "Totalbelopp",
                table: "Bestallning",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");
        }
    }
}
