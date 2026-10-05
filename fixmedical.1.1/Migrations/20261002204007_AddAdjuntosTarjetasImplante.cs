using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AddAdjuntosTarjetasImplante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTA: se eliminaron los DropForeignKey/DropIndex/DropColumn de
            // TarjetaID_TarjetaImplante y el AddForeignKey duplicado, porque esas
            // operaciones ya se ejecutaron manualmente en la BD y volver a
            // ejecutarlas produciría error.

            migrationBuilder.CreateTable(
                name: "TarjetasImplanteAdjuntos",
                columns: table => new
                {
                    ID_TarjetasImplanteAdjuntos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_TarjetaImplante = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NombreOriginal = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarjetasImplanteAdjuntos", x => x.ID_TarjetasImplanteAdjuntos);
                    table.ForeignKey(
                        name: "FK_TarjetasImplanteAdjuntos_TarjetasImplante_ID_TarjetaImplante",
                        column: x => x.ID_TarjetaImplante,
                        principalTable: "TarjetasImplante",
                        principalColumn: "ID_TarjetaImplante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasImplanteDetalle_ID_TarjetaImplante",
                table: "TarjetasImplanteDetalle",
                column: "ID_TarjetaImplante");

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasImplanteAdjuntos_ID_TarjetaImplante",
                table: "TarjetasImplanteAdjuntos",
                column: "ID_TarjetaImplante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TarjetasImplanteAdjuntos_ID_TarjetaImplante",
                table: "TarjetasImplanteAdjuntos");

            migrationBuilder.DropTable(
                name: "TarjetasImplanteAdjuntos");

            migrationBuilder.DropIndex(
                name: "IX_TarjetasImplanteDetalle_ID_TarjetaImplante",
                table: "TarjetasImplanteDetalle");
        }
    }
}