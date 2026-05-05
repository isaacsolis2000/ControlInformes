using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlInformes.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    IdAsistencia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaReunion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoReunion = table.Column<int>(type: "int", nullable: true),
                    CantidadPresencial = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CantidadVirtual = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.IdAsistencia);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    IdGrupo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdCapitan = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.IdGrupo);
                });

            migrationBuilder.CreateTable(
                name: "Publicadores",
                columns: table => new
                {
                    IdPublicador = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaBautismo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicadores", x => x.IdPublicador);
                    table.ForeignKey(
                        name: "FK_Publicadores_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "IdGrupo",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InformesMensuales",
                columns: table => new
                {
                    IdInformeMensual = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPublicador = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Participo = table.Column<bool>(type: "bit", nullable: false),
                    CursosBiblicos = table.Column<int>(type: "int", nullable: false),
                    Horas = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Inactivo = table.Column<bool>(type: "bit", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformesMensuales", x => x.IdInformeMensual);
                    table.ForeignKey(
                        name: "FK_InformesMensuales_Publicadores_IdPublicador",
                        column: x => x.IdPublicador,
                        principalTable: "Publicadores",
                        principalColumn: "IdPublicador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_FechaReunion_TipoReunion",
                table: "Asistencias",
                columns: new[] { "FechaReunion", "TipoReunion" },
                unique: true,
                filter: "[TipoReunion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_IdCapitan",
                table: "Grupos",
                column: "IdCapitan");

            migrationBuilder.CreateIndex(
                name: "IX_InformesMensuales_IdPublicador_Ano_Mes",
                table: "InformesMensuales",
                columns: new[] { "IdPublicador", "Ano", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publicadores_IdGrupo",
                table: "Publicadores",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Grupos_Publicadores_IdCapitan",
                table: "Grupos",
                column: "IdCapitan",
                principalTable: "Publicadores",
                principalColumn: "IdPublicador",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grupos_Publicadores_IdCapitan",
                table: "Grupos");

            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "InformesMensuales");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Publicadores");

            migrationBuilder.DropTable(
                name: "Grupos");
        }
    }
}
