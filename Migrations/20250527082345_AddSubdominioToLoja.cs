using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitrineApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSubdominioToLoja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subdominio",
                table: "Loja",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subdominio",
                table: "Loja");
        }
    }
}
