using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHistorialActo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CirugiaSnapshot",
                table: "HojasGasto",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CirujanoSnapshot",
                table: "HojasGasto",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FechaHoraFin",
                table: "HojasGasto",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FechaHoraInicio",
                table: "HojasGasto",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ID_Programacion",
                table: "HojasGasto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LatitudFin",
                table: "HojasGasto",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LatitudInicio",
                table: "HojasGasto",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LongitudFin",
                table: "HojasGasto",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LongitudInicio",
                table: "HojasGasto",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RutaPdf",
                table: "HojasGasto",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CirugiaSnapshot",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "CirujanoSnapshot",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "FechaHoraFin",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "FechaHoraInicio",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "ID_Programacion",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "LatitudFin",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "LatitudInicio",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "LongitudFin",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "LongitudInicio",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "RutaPdf",
                table: "HojasGasto");
        }
    }
}
