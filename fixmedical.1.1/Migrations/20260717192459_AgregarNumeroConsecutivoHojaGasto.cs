using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNumeroConsecutivoHojaGasto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HojasGasto",
                columns: table => new
                {
                    ID_HojaGasto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroConsecutivo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Especialista = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Paciente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PacienteID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Linea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fabricante = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DomicilioPais = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HojasGasto", x => x.ID_HojaGasto);
                });

            migrationBuilder.CreateTable(
                name: "HojasGastoDetalle",
                columns: table => new
                {
                    ID_Detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_HojaGasto = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegInvima = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HojasGastoDetalle", x => x.ID_Detalle);
                    table.ForeignKey(
                        name: "FK_HojasGastoDetalle_HojasGasto_ID_HojaGasto",
                        column: x => x.ID_HojaGasto,
                        principalTable: "HojasGasto",
                        principalColumn: "ID_HojaGasto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HojasGasto_NumeroConsecutivo",
                table: "HojasGasto",
                column: "NumeroConsecutivo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HojasGastoDetalle_ID_HojaGasto",
                table: "HojasGastoDetalle",
                column: "ID_HojaGasto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HojasGastoDetalle");

            migrationBuilder.DropTable(
                name: "HojasGasto");
        }
    }
}
