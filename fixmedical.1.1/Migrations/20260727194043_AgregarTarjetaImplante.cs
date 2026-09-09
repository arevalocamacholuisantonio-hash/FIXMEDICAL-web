using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTarjetaImplante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarjetasImplante",
                columns: table => new
                {
                    ID_TarjetaImplante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroConsecutivo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Remision = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaCirugia = table.Column<DateTime>(type: "datetime2", maxLength: 150, nullable: false),
                    NombrePaciente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NombreEspecialista = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NombreEQC = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NombreIqxInstitucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FirmaEspecialista = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    firmaEQC = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FirmaIqxInstitucion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Fabricante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RutaStiker = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RutaPdf = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LatitudInicio = table.Column<double>(type: "float", nullable: true),
                    LongitudInicio = table.Column<double>(type: "float", nullable: true),
                    FechaHoraInicio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LatitudFin = table.Column<double>(type: "float", nullable: true),
                    LongitudFin = table.Column<double>(type: "float", nullable: true),
                    FechaHoraFin = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DuracionSegundos = table.Column<int>(type: "int", nullable: false),
                    ID_Programacion = table.Column<int>(type: "int", nullable: true),
                    CirujanoSnapshot = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CirugiaSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarjetasImplante", x => x.ID_TarjetaImplante);
                });

            migrationBuilder.CreateTable(
                name: "TarjetasImplanteDetalle",
                columns: table => new
                {
                    ID_TarjetaImplanteDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_TarjetaImplante = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Referencias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegInvima = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TarjetaID_TarjetaImplante = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarjetasImplanteDetalle", x => x.ID_TarjetaImplanteDetalle);
                    table.ForeignKey(
                        name: "FK_TarjetasImplanteDetalle_TarjetasImplante_TarjetaID_TarjetaImplante",
                        column: x => x.TarjetaID_TarjetaImplante,
                        principalTable: "TarjetasImplante",
                        principalColumn: "ID_TarjetaImplante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasImplanteDetalle_TarjetaID_TarjetaImplante",
                table: "TarjetasImplanteDetalle",
                column: "TarjetaID_TarjetaImplante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarjetasImplanteDetalle");

            migrationBuilder.DropTable(
                name: "TarjetasImplante");
        }
    }
}
