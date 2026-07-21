using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace fixmedical._1._1.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicialFixmedical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especialistas",
                columns: table => new
                {
                    ID_Especialista = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Especialista = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialistas", x => x.ID_Especialista);
                });

            migrationBuilder.CreateTable(
                name: "Instituciones",
                columns: table => new
                {
                    ID_Institucion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Institucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituciones", x => x.ID_Institucion);
                });

            migrationBuilder.CreateTable(
                name: "Lineas",
                columns: table => new
                {
                    ID_Linea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Linea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineas", x => x.ID_Linea);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    ID_Paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Documento_Identidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre_Completo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Edad_Registro = table.Column<int>(type: "int", nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.ID_Paciente);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID_Rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID_Rol);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Codigo_Fix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Nombre_Generico = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Registro_Invima = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fecha_Vencimiento_Invima = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Fabricante = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Pais_Origen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Clasificacion_Riesgo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Tipo_Producto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ID_Linea = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Codigo_Fix);
                    table.ForeignKey(
                        name: "FK_Productos_Lineas_ID_Linea",
                        column: x => x.ID_Linea,
                        principalTable: "Lineas",
                        principalColumn: "ID_Linea");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    ID_Usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Usuario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasena_Hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ID_Rol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ultimo_Acceso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.ID_Usuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_ID_Rol",
                        column: x => x.ID_Rol,
                        principalTable: "Roles",
                        principalColumn: "ID_Rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesCambios",
                columns: table => new
                {
                    ID_Log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tabla_Afectada = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_Registro_Afectado = table.Column<int>(type: "int", nullable: false),
                    Fecha_Hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion_Cambio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesCambios", x => x.ID_Log);
                    table.ForeignKey(
                        name: "FK_HistorialesCambios_Usuarios_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuarios",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InformacionesPacientes",
                columns: table => new
                {
                    ID_Registro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Paciente = table.Column<int>(type: "int", nullable: false),
                    Consecutivo_Gasto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fecha_Cirugia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_Institucion = table.Column<int>(type: "int", nullable: false),
                    ID_Especialista = table.Column<int>(type: "int", nullable: false),
                    ID_Usuario_Registra = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformacionesPacientes", x => x.ID_Registro);
                    table.ForeignKey(
                        name: "FK_InformacionesPacientes_Especialistas_ID_Especialista",
                        column: x => x.ID_Especialista,
                        principalTable: "Especialistas",
                        principalColumn: "ID_Especialista",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InformacionesPacientes_Instituciones_ID_Institucion",
                        column: x => x.ID_Institucion,
                        principalTable: "Instituciones",
                        principalColumn: "ID_Institucion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InformacionesPacientes_Pacientes_ID_Paciente",
                        column: x => x.ID_Paciente,
                        principalTable: "Pacientes",
                        principalColumn: "ID_Paciente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InformacionesPacientes_Usuarios_ID_Usuario_Registra",
                        column: x => x.ID_Usuario_Registra,
                        principalTable: "Usuarios",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosConsumos",
                columns: table => new
                {
                    ID_Consumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Registro = table.Column<int>(type: "int", nullable: false),
                    Codigo_Fix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosConsumos", x => x.ID_Consumo);
                    table.ForeignKey(
                        name: "FK_RegistrosConsumos_InformacionesPacientes_ID_Registro",
                        column: x => x.ID_Registro,
                        principalTable: "InformacionesPacientes",
                        principalColumn: "ID_Registro",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosConsumos_Productos_Codigo_Fix",
                        column: x => x.Codigo_Fix,
                        principalTable: "Productos",
                        principalColumn: "Codigo_Fix",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID_Rol", "Descripcion", "Nombre_Rol" },
                values: new object[,]
                {
                    { 1, "Acceso total. Administra usuarios, catálogo y auditoría.", "Administrador" },
                    { 2, "Especialista Quirúrgico. Registra pacientes, cirugías y consumos.", "EQC" },
                    { 3, "Solo lectura. Genera reportes y consulta consumos.", "Auditor" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesCambios_ID_Usuario",
                table: "HistorialesCambios",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_InformacionesPacientes_Consecutivo_Gasto",
                table: "InformacionesPacientes",
                column: "Consecutivo_Gasto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InformacionesPacientes_ID_Especialista",
                table: "InformacionesPacientes",
                column: "ID_Especialista");

            migrationBuilder.CreateIndex(
                name: "IX_InformacionesPacientes_ID_Institucion",
                table: "InformacionesPacientes",
                column: "ID_Institucion");

            migrationBuilder.CreateIndex(
                name: "IX_InformacionesPacientes_ID_Paciente",
                table: "InformacionesPacientes",
                column: "ID_Paciente");

            migrationBuilder.CreateIndex(
                name: "IX_InformacionesPacientes_ID_Usuario_Registra",
                table: "InformacionesPacientes",
                column: "ID_Usuario_Registra");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ID_Linea",
                table: "Productos",
                column: "ID_Linea");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosConsumos_Codigo_Fix",
                table: "RegistrosConsumos",
                column: "Codigo_Fix");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosConsumos_ID_Registro",
                table: "RegistrosConsumos",
                column: "ID_Registro");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ID_Rol",
                table: "Usuarios",
                column: "ID_Rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialesCambios");

            migrationBuilder.DropTable(
                name: "RegistrosConsumos");

            migrationBuilder.DropTable(
                name: "InformacionesPacientes");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Especialistas");

            migrationBuilder.DropTable(
                name: "Instituciones");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Lineas");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
