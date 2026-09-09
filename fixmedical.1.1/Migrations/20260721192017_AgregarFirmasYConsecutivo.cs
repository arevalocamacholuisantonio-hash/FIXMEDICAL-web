using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFirmasYConsecutivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Linea",
                table: "HojasGasto",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DomicilioPais",
                table: "HojasGasto",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "FirmaEspecialista",
                table: "HojasGasto",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirmaFixMedical",
                table: "HojasGasto",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirmaInstitucion",
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
                name: "FirmaEspecialista",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "FirmaFixMedical",
                table: "HojasGasto");

            migrationBuilder.DropColumn(
                name: "FirmaInstitucion",
                table: "HojasGasto");

            migrationBuilder.AlterColumn<string>(
                name: "Linea",
                table: "HojasGasto",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DomicilioPais",
                table: "HojasGasto",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
