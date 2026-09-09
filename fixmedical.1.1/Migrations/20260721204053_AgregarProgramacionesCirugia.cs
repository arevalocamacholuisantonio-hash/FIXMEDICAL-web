using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProgramacionesCirugia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramacionesCirugia",
                columns: table => new
                {
                    ID_Programacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cirujano = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CirugiaProgramada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramacionesCirugia", x => x.ID_Programacion);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramacionesCirugia");
        }
    }
}
