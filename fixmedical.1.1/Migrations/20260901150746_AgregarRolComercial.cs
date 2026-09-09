using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRolComercial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ID_EQC_Asignado",
                table: "ProgramacionesCirugia",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ID_SolicitudMaterial",
                table: "ProgramacionesCirugia",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SolicitudesMaterial",
                columns: table => new
                {
                    ID_SolicitudMaterial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroSolicitud = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoSolicitud = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "date", nullable: false),
                    HoraSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sucursal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EjecutivoComercial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Facturar_A = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Especialista = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaCirugia = table.Column<DateTime>(type: "date", nullable: false),
                    HoraCirugia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombrePaciente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NumeroIdentificacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Especialidad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Indicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaterialSolicitado = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Motor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ObservacionesEjecutivos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesMaterial", x => x.ID_SolicitudMaterial);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID_Rol", "Descripcion", "Nombre_Rol" },
                values: new object[] { 4, "Genera solicitudes de material de cirugia .", "Comercial" });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesCirugia_ID_EQC_Asignado",
                table: "ProgramacionesCirugia",
                column: "ID_EQC_Asignado");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramacionesCirugia_ID_SolicitudMaterial",
                table: "ProgramacionesCirugia",
                column: "ID_SolicitudMaterial");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramacionesCirugia_SolicitudesMaterial_ID_SolicitudMaterial",
                table: "ProgramacionesCirugia",
                column: "ID_SolicitudMaterial",
                principalTable: "SolicitudesMaterial",
                principalColumn: "ID_SolicitudMaterial");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramacionesCirugia_Usuarios_ID_EQC_Asignado",
                table: "ProgramacionesCirugia",
                column: "ID_EQC_Asignado",
                principalTable: "Usuarios",
                principalColumn: "ID_Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramacionesCirugia_SolicitudesMaterial_ID_SolicitudMaterial",
                table: "ProgramacionesCirugia");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgramacionesCirugia_Usuarios_ID_EQC_Asignado",
                table: "ProgramacionesCirugia");

            migrationBuilder.DropTable(
                name: "SolicitudesMaterial");

            migrationBuilder.DropIndex(
                name: "IX_ProgramacionesCirugia_ID_EQC_Asignado",
                table: "ProgramacionesCirugia");

            migrationBuilder.DropIndex(
                name: "IX_ProgramacionesCirugia_ID_SolicitudMaterial",
                table: "ProgramacionesCirugia");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "ID_Rol",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "ID_EQC_Asignado",
                table: "ProgramacionesCirugia");

            migrationBuilder.DropColumn(
                name: "ID_SolicitudMaterial",
                table: "ProgramacionesCirugia");
        }
    }
}
