using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDuracionActo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracionSegundos",
                table: "HojasGasto",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionSegundos",
                table: "HojasGasto");
        }
    }
}
