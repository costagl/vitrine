namespace VitrineApi.DTOs
{
    public class LojaDto
    {
        // O ID continua obrigatório (int normal) pois é a chave de segurança
        public int Id { get; set; }

        // Campos tornados anuláveis (?) para permitir o PATCH parcial
        public string? Subdominio { get; set; }
        public string? NomeLoja { get; set; }

        // Importante: int? permite saber se o campo não foi enviado (null) ou se é 0
        public int? IdTema { get; set; }
        public int? IdLayout { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cpf_Cnpj { get; set; }
        public decimal? Avaliacao { get; set; }
        public string? Descricao { get; set; }
        public string? LogotipoUrl { get; set; }
        public string? ImagemBannerUrl { get; set; }

        // Campos de leitura (vindos de joins)
        public string? CategoriaLoja { get; set; }
        public string? TituloTema { get; set; }
        public string? TituloLayout { get; set; }

        // Objetos aninhados também precisam ser opcionais
        public LojistaDto? Lojista { get; set; }

        // Listas opcionais (para não precisar enviá-las vazias no Patch)
        public List<ProdutoDto>? Produtos { get; set; }
        public List<CategoriaProdutoDto>? CategoriasProduto { get; set; }
    }

    public class LojistaDto
    {
        public string? NomeCompleto { get; set; }

        // DateOnly? permite enviar null se não quiser alterar a data
        public DateOnly? DataNascimento { get; set; }

        public string? Email { get; set; }
        public string? Telefone { get; set; }

        // Endereço opcional
        public EnderecoLojistaDto? Endereco { get; set; }
    }

    public class EnderecoLojistaDto
    {
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
    }

    public class ProdutoDto
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal? ValorPromocional { get; set; }
        public int Estoque { get; set; }
        public string? Descricao { get; set; }
        public string? CategoriaProduto { get; set; }
        public string? ImagemUrl { get; set; }
        public string? Sku { get; set; }
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
        public int IdCategoriaProduto { get; set; }
        public string? TituloCategoriaProduto { get; set; }
    }

    // Como integramos a lógica no Patch, essa classe pode ficar obsoleta,
    // mas mantive aqui caso você ainda a use em outro lugar.
    public class AlterarLayoutTemaDto
    {
        public int? NovoLayoutId { get; set; }
        public int? NovoTemaId { get; set; }
    }
}