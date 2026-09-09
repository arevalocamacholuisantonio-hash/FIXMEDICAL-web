using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class RutasArchivosTarjeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirmaEQCPath",
                table: "TarjetasImplante",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmaEspPath",
                table: "TarjetasImplante",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmaIQXPath",
                table: "TarjetasImplante",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "TarjetasImplante",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirmaEQCPath",
                table: "TarjetasImplante");

            migrationBuilder.DropColumn(
                name: "FirmaEspPath",
                table: "TarjetasImplante");

            migrationBuilder.DropColumn(
                name: "FirmaIQXPath",
                table: "TarjetasImplante");

            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "TarjetasImplante");
        }
    }
}
