using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlInformes.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    IdAsistencia = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoReunion = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.IdAsistencia);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    IdGrupo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capitan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
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
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBautismo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicadores", x => x.IdPublicador);
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
                    Tipo = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PublicadorGrupos",
                columns: table => new
                {
                    IdPublicadorGrupo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPublicador = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdGrupo = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicadorGrupos", x => x.IdPublicadorGrupo);
                    table.ForeignKey(
                        name: "FK_PublicadorGrupos_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "IdGrupo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicadorGrupos_Publicadores_IdPublicador",
                        column: x => x.IdPublicador,
                        principalTable: "Publicadores",
                        principalColumn: "IdPublicador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InformesMensuales_IdPublicador_Ano_Mes",
                table: "InformesMensuales",
                columns: new[] { "IdPublicador", "Ano", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicadorGrupos_IdGrupo",
                table: "PublicadorGrupos",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_PublicadorGrupos_IdPublicador",
                table: "PublicadorGrupos",
                column: "IdPublicador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "InformesMensuales");

            migrationBuilder.DropTable(
                name: "PublicadorGrupos");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Publicadores");
        }
    }
}
