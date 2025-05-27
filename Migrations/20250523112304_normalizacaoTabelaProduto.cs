using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitrineApi.Migrations
{
    /// <inheritdoc />
    public partial class normalizacaoTabelaProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descrição",
                table: "Produto",
                newName: "Descricao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Produto",
                newName: "Descrição");
        }
    }
}
