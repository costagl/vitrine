namespace VitrineApi.DTOs
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int IdLoja { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal? ValorPromocional { get; set; }
        public int Estoque { get; set; }
        public string Sku { get; set; }
        public string Imagem { get; set; }
        public byte Ativo { get; set; }
        public decimal Peso { get; set; }
        public string Descricao { get; set; }
        public decimal Altura { get; set; }
        public decimal Largura { get; set; }
        public decimal Profundidade { get; set; }
        public int IdCategoriaProduto { get; set; }
        public string NomeCategoriaProduto { get; set; }
    }
}
