using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitrineApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusPedidoEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnderecoEntrega_Cliente",
                table: "EnderecoEntrega");

            migrationBuilder.DropForeignKey(
                name: "FK_Loja_Lojista",
                table: "Loja");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Cliente",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lojista",
                table: "Lojista");

            migrationBuilder.DropIndex(
                name: "IX_Loja_Cpf",
                table: "Loja");

            migrationBuilder.DropIndex(
                name: "IX_EnderecoEntrega_IdCliente",
                table: "EnderecoEntrega");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Lojista");

            migrationBuilder.DropColumn(
                name: "CategoriaLoja",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "NomeProduto",
                table: "ItensPedido");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "EnderecoEntrega");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Cliente");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "Pedido",
                newName: "DataPedido");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "ItensPedido",
                newName: "PrecoTotal");

            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "AspNetUsers",
                newName: "Cpf_Cnpj");

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                table: "Produto",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produto",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Pedido",
                type: "int",
                unicode: false,
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30);

            migrationBuilder.AddColumn<string>(
                name: "CpfCliente",
                table: "Pedido",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cpf_Cnpj",
                table: "Lojista",
                type: "varchar(14)",
                unicode: false,
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Avaliacao",
                table: "Loja",
                type: "decimal(3,1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf_Cnpj",
                table: "Loja",
                type: "varchar(14)",
                unicode: false,
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Loja",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCategoria",
                table: "Loja",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Logotipo",
                table: "Loja",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "EnderecoEntrega",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Logradouro",
                table: "EnderecoEntrega",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "EnderecoEntrega",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cidade",
                table: "EnderecoEntrega",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cep",
                table: "EnderecoEntrega",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bairro",
                table: "EnderecoEntrega",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CpfCliente",
                table: "EnderecoEntrega",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Cliente",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "CategoriaProduto",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Imagem",
                table: "CategoriaProduto",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CategoriaProduto",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "IdCategoriaLoja",
                table: "CategoriaProduto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lojista",
                table: "Lojista",
                column: "Cpf_Cnpj");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cliente_1",
                table: "Cliente",
                column: "Cpf");

            migrationBuilder.CreateTable(
                name: "CategoriaLoja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Imagem = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaLoja", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido",
                column: "CpfCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Loja_Cpf",
                table: "Loja",
                column: "Cpf_Cnpj");

            migrationBuilder.CreateIndex(
                name: "IX_Loja_IdCategoria",
                table: "Loja",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_EnderecoEntrega_IdCliente",
                table: "EnderecoEntrega",
                column: "CpfCliente");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaProduto_IdCategoriaLoja",
                table: "CategoriaProduto",
                column: "IdCategoriaLoja");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriaProduto_CategoriaLoja",
                table: "CategoriaProduto",
                column: "IdCategoriaLoja",
                principalTable: "CategoriaLoja",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EnderecoEntrega_Cliente1",
                table: "EnderecoEntrega",
                column: "CpfCliente",
                principalTable: "Cliente",
                principalColumn: "Cpf");

            migrationBuilder.AddForeignKey(
                name: "FK_Loja_CategoriaLoja",
                table: "Loja",
                column: "IdCategoria",
                principalTable: "CategoriaLoja",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loja_Lojista",
                table: "Loja",
                column: "Cpf_Cnpj",
                principalTable: "Lojista",
                principalColumn: "Cpf_Cnpj");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Cliente1",
                table: "Pedido",
                column: "CpfCliente",
                principalTable: "Cliente",
                principalColumn: "Cpf");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriaProduto_CategoriaLoja",
                table: "CategoriaProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_EnderecoEntrega_Cliente1",
                table: "EnderecoEntrega");

            migrationBuilder.DropForeignKey(
                name: "FK_Loja_CategoriaLoja",
                table: "Loja");

            migrationBuilder.DropForeignKey(
                name: "FK_Loja_Lojista",
                table: "Loja");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Cliente1",
                table: "Pedido");

            migrationBuilder.DropTable(
                name: "CategoriaLoja");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lojista",
                table: "Lojista");

            migrationBuilder.DropIndex(
                name: "IX_Loja_Cpf",
                table: "Loja");

            migrationBuilder.DropIndex(
                name: "IX_Loja_IdCategoria",
                table: "Loja");

            migrationBuilder.DropIndex(
                name: "IX_EnderecoEntrega_IdCliente",
                table: "EnderecoEntrega");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cliente_1",
                table: "Cliente");

            migrationBuilder.DropIndex(
                name: "IX_CategoriaProduto_IdCategoriaLoja",
                table: "CategoriaProduto");

            migrationBuilder.DropColumn(
                name: "CpfCliente",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Cpf_Cnpj",
                table: "Lojista");

            migrationBuilder.DropColumn(
                name: "Avaliacao",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "Cpf_Cnpj",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "IdCategoria",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "Logotipo",
                table: "Loja");

            migrationBuilder.DropColumn(
                name: "CpfCliente",
                table: "EnderecoEntrega");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "IdCategoriaLoja",
                table: "CategoriaProduto");

            migrationBuilder.RenameColumn(
                name: "DataPedido",
                table: "Pedido",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "PrecoTotal",
                table: "ItensPedido",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "Cpf_Cnpj",
                table: "AspNetUsers",
                newName: "Cpf");

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                table: "Produto",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produto",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Pedido",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldUnicode: false,
                oldMaxLength: 30);

            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "Pedido",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Lojista",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaLoja",
                table: "Loja",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "Loja",
                type: "varchar(14)",
                unicode: false,
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Loja",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeProduto",
                table: "ItensPedido",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "EnderecoEntrega",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "Logradouro",
                table: "EnderecoEntrega",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "EnderecoEntrega",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Cidade",
                table: "EnderecoEntrega",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Cep",
                table: "EnderecoEntrega",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Bairro",
                table: "EnderecoEntrega",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "EnderecoEntrega",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Cliente",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "CategoriaProduto",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                table: "CategoriaProduto",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CategoriaProduto",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lojista",
                table: "Lojista",
                column: "Cpf");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Loja_Cpf",
                table: "Loja",
                column: "Cpf");

            migrationBuilder.CreateIndex(
                name: "IX_EnderecoEntrega_IdCliente",
                table: "EnderecoEntrega",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_EnderecoEntrega_Cliente",
                table: "EnderecoEntrega",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loja_Lojista",
                table: "Loja",
                column: "Cpf",
                principalTable: "Lojista",
                principalColumn: "Cpf");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Cliente",
                table: "Pedido",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "Id");
        }
    }
}
