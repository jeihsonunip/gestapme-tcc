using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaPME.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioAtivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Reativa usuários que existiam antes desta migration
            migrationBuilder.Sql("UPDATE [Usuarios] SET [Ativo] = 1 WHERE [Ativo] = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Usuarios");
        }
    }
}
