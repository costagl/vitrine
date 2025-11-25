namespace VitrineApi.DTOs
{
    public class LojaDto
    {
        public int Id { get; set; }
        public string Subdominio { get; set; }
        public string NomeLoja { get; set; }
        public int IdTema { get; set; }
        public int IdLayout { get; set; }
        public string Cpf_Cnpj { get; set; }
        public decimal Avaliacao { get; set; }
        public string Descricao { get; set; }
        public string LogotipoUrl { get; set; }
        public string CategoriaLoja { get; set; }
        public string TituloTema { get; set; }
        public string TituloLayout { get; set; }
        public LojistaDto Lojista { get; set; }
        public List<ProdutoDto> Produtos { get; set; }
        public List<CategoriaProdutoDto> CategoriasProduto { get; set; }  // Novo campo para categorias de produto
    }

    public class LojistaDto
    {
        public string NomeCompleto { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }

    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal? ValorPromocional { get; set; }
        public int Estoque { get; set; }
        public string Descricao { get; set; }
        public string CategoriaProduto { get; set; }
        public string ImagemUrl { get; set; }
        public string Sku { get; set; }
        public byte Ativo { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal Largura { get; set; }
        public decimal Profundidade { get; set; }
        public decimal? ValorCusto { get; set; }
        public int IdCategoriaProduto { get; set; }
        public int IdLoja { get; set; }
    }

    public class CategoriaProdutoDto
    {
        public int IdCategoriaProduto { get; set; }  // ID da categoria de produto
        public string TituloCategoriaProduto { get; set; }  // Título da categoria de produto
    }

    // Alterar Layout/Tema
    public class AlterarLayoutTemaRequest
    {
        public int? NovoLayoutId { get; set; }
        public int? NovoTemaId { get; set; }
    }
}




