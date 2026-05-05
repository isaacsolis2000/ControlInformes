using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlInformes.Data.Migrations
{
    /// <inheritdoc />
    public partial class NuevoCamposPublicador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CondicionEspiritual",
                table: "Publicadores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Genero",
                table: "Publicadores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rol",
                table: "Publicadores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CondicionEspiritual",
                table: "Publicadores");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Publicadores");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Publicadores");
        }
    }
}
